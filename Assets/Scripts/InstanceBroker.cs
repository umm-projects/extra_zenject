using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace ExtraZenject
{
    public interface IInstanceBroker
    {
        void Declare<T>();
        void Release<T>();
        void Publish<T>(T instance);
        IObservable<T> Receive<T>();
    }

    public class InstanceBroker : IInstanceBroker
    {
        public enum InjectId
        {
            BrokableInstanceType,
        }

        private IDictionary<Type, ISubject<object>> SubjectMap { get; } = new Dictionary<Type, ISubject<object>>();

        [Inject]
        public void Construct(
            [Inject(Id = InjectId.BrokableInstanceType)] IEnumerable<Type> brokeInstanceTypes
        )
        {
            foreach (var type in brokeInstanceTypes)
            {
                DeclareInternal(type);
            }
        }

        public void Declare<T>()
        {
            DeclareInternal(typeof(T));
        }

        public void Release<T>()
        {
            ReleaseInternal(typeof(T));
        }

        public void Publish<T>(T instance)
        {
            PublishInternal(instance, true);
        }

        public void TryPublish<T>(T instance)
        {
            PublishInternal(instance, false);
        }

        public IObservable<T> Receive<T>()
        {
            return ReceiveInternal<T>(true);
        }

        public IObservable<T> TryReceive<T>()
        {
            return ReceiveInternal<T>(false);
        }

        private void DeclareInternal(Type type)
        {
            if (SubjectMap.ContainsKey(type))
            {
                ((ReplaySubject<object>) SubjectMap[type]).Dispose();
            }

            SubjectMap[type] = new ReplaySubject<object>();
        }

        private void ReleaseInternal(Type type)
        {
            if (!SubjectMap.ContainsKey(type))
            {
                Debug.LogWarning($"`{type}' does not declared.");
                return;
            }

            SubjectMap.Remove(type);
        }

        private void PublishInternal<T>(T instance, bool assert)
        {
            if (!SubjectMap.ContainsKey(typeof(T)))
            {
                if (assert)
                {
                    throw new KeyNotFoundException($"`{typeof(T)}' has not declared by DiContainer.DeclareBrokableInstance(). Please call `Container.DeclareBrokableInstance<{typeof(T)}>() in installer.'");
                }

                return;
            }

            SubjectMap[typeof(T)].OnNext(instance);
        }

        private IObservable<T> ReceiveInternal<T>(bool assert)
        {
            if (!SubjectMap.ContainsKey(typeof(T)))
            {
                if (assert)
                {
                    throw new KeyNotFoundException($"`{typeof(T)}' has not declared by DiContainer.DeclareBrokableInstance(). Please call `Container.DeclareBrokableInstance<{typeof(T)}>() in installer.");
                }

                return Observable.Never<T>();
            }

            return SubjectMap[typeof(T)]
                .Where(x => (!(x is Object) && x != null) || (Object) x != null)
                .Select(x => (T) x);
        }
    }

    public static class InstanceBrokerExtension
    {
        public static void DeclareBrokableInstance<T>(this DiContainer container)
        {
            if (container.HasBinding<IInstanceBroker>())
            {
                var instanceBroker = ProjectContext.Instance.Container.Resolve<IInstanceBroker>();
                instanceBroker.Declare<T>();
            }
            else
            {
                container.BindInstance(typeof(T)).WithId(InstanceBroker.InjectId.BrokableInstanceType);
            }
        }
    }
}
