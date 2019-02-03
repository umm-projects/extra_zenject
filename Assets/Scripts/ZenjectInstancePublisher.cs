using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ExtraZenject
{
    [RequireComponent(typeof(ZenjectBinding))]
    public class ZenjectInstancePublisher : MonoBehaviour
    {
        private ZenjectBinding cachedZenjectBinding;
        private ZenjectBinding ZenjectBinding => cachedZenjectBinding ? cachedZenjectBinding : (cachedZenjectBinding = GetComponent<ZenjectBinding>());

        private static IInstanceBroker Publisher { get; set; }

        private void Awake()
        {
            if (Publisher == default)
            {
                Publisher = ProjectContext.Instance.Container.Resolve<IInstanceBroker>();
            }
        }

        private void Start()
        {
            foreach (var component in ZenjectBinding.Components)
            {
                switch (ZenjectBinding.BindType)
                {
                    case ZenjectBinding.BindTypes.Self:
                        InvokeFire(component.GetType(), component);
                        break;
                    case ZenjectBinding.BindTypes.AllInterfaces:
                        component
                            .GetType()
                            .GetInterfaces()
                            .ToList()
                            .ForEach(type => InvokeFire(type, component));
                        break;
                    case ZenjectBinding.BindTypes.AllInterfacesAndSelf:
                        component
                            .GetType()
                            .GetInterfaces()
                            .ToList()
                            .ForEach(type => InvokeFire(type, component));
                        InvokeFire(component.GetType(), component);
                        break;
                    case ZenjectBinding.BindTypes.BaseType:
                        if (component.GetType().BaseType != default)
                        {
                            InvokeFire(component.GetType().BaseType, component);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void InvokeFire(Type type, object instance)
        {
            Publisher
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(
                    mi =>
                        mi.Name == "TryPublish"
                        && mi.IsGenericMethod
                        && mi.IsGenericMethodDefinition
                        && mi.ContainsGenericParameters
                        && mi.GetParameters().Length == 1
                )
                .MakeGenericMethod(type)
                .Invoke(
                    Publisher,
                    new[]
                    {
                        instance
                    }
                );
        }
    }
}