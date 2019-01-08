using System;
using UniRx;
using Zenject;

namespace ExtraZenject
{
    public interface ISpawnedInstanceReceiver
    {
        // ReSharper disable once UnusedMemberInSuper.Global
        SignalBus Receiver { get; set; }
    }

    public static class SpawnedInstanceReceiverExtension
    {
        public static IObservable<T> OnSpawnAsObservable<T>(this ISpawnedInstanceReceiver self)
        {
            return self.Receiver.GetStream<SpawnedInstance<T>>().Select(x => x.Instance);
        }
    }
}
