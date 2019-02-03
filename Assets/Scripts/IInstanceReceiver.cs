using System;
using Zenject;

namespace ExtraZenject
{
    public interface IInstanceReceiver
    {
    }

    public static class InstanceReceiverExtension
    {
        public static IObservable<T> Receive<T>(this IInstanceReceiver self)
        {
            return ProjectContext.Instance.Container.Resolve<IInstanceBroker>().Receive<T>();
        }
    }
}
