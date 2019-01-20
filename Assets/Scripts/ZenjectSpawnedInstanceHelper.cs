using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ExtraZenject
{
    [RequireComponent(typeof(ZenjectBinding))]
    public class ZenjectSpawnedInstanceHelper : MonoBehaviour
    {
        private ZenjectBinding cachedZenjectBinding;
        private ZenjectBinding ZenjectBinding => cachedZenjectBinding ? cachedZenjectBinding : (cachedZenjectBinding = GetComponent<ZenjectBinding>());

        private SignalBus SignalBus { get; set; }

        private static Type SpawnedInstanceType { get; } = typeof(SpawnedInstance<>);

        private void Start()
        {
            SignalBus =
                (
                    // Resolve SignalBus from SceneContext to match context if ZenjectBinding.UseSceneContext is true
                    ZenjectBinding.UseSceneContext
                        ? gameObject
                            .scene
                            // Expect that there is a SceneContext in Root
                            .GetRootGameObjects()
                            .First(x => x.GetComponent<SceneContext>() != null)
                            .GetComponent<SceneContext>()
                            .Container
                        : ProjectContext.Instance.Container
                )
                .Resolve<SignalBus>();
            foreach (var component in ZenjectBinding.Components)
            {
                switch (ZenjectBinding.BindType)
                {
                    case ZenjectBinding.BindTypes.Self:
                        SignalBus.TryFire(component.GetType(), component);
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
                        SignalBus.TryFire(component.GetType(), component);
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

        private void InvokeFire(Type type, object instance)
        {
            var targetType = SpawnedInstanceType.MakeGenericType(type);
            SignalBus
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(
                    mi =>
                        mi.Name == "TryFire"
                        && mi.IsGenericMethod
                        && mi.IsGenericMethodDefinition
                        && mi.ContainsGenericParameters
                        && mi.GetParameters().Length == 2
                )
                .MakeGenericMethod(targetType)
                .Invoke(
                    SignalBus,
                    new[]
                    {
                        Activator.CreateInstance(targetType, instance),
                        string.IsNullOrEmpty(ZenjectBinding.Identifier) ? null : ZenjectBinding.Identifier
                    }
                );
        }
    }
}