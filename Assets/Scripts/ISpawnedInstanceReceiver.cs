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
            self.Receiver.GetStream<SpawnedInstance<T>>().Subscribe(_ => UnityEngine.Debug.Log(_));
            return self.Receiver.GetStream<SpawnedInstance<T>>().Select(x => x.Instance);
        }
    }
}