using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace ExtraZenject
{
    public interface ISpawnedInstanceReceiver
    {
        // ReSharper disable once UnusedMemberInSuper.Global
        SignalBus Receiver { get; set; }
    }

    public interface IStorableSpawnedInstanceReceiver<T> : ISpawnedInstanceReceiver
    {
        IList<T> InstanceList { get; }
    }

    public static class SpawnedInstanceReceiverExtension
    {
        public static IObservable<T> OnSpawnAsObservable<T>(this ISpawnedInstanceReceiver self)
        {
            return self.Receiver.GetStream<SpawnedInstance<T>>().Select(x => x.Instance);
        }

        public static IDisposable SubscribeSpawn<T>(this IStorableSpawnedInstanceReceiver<T> self)
        {
            return self
                .OnSpawnAsObservable<T>()
                .Subscribe(
                    x =>
                    {
                        self.InstanceList.Add(x);
                        if (x is MonoBehaviour monoBehaviour)
                        {
                            monoBehaviour
                                .OnDestroyAsObservable()
                                .Subscribe(
                                    _ => self
                                        .InstanceList
                                        .Remove(x)
                                );
                        }
                    }
                );
        }

        public static void InvokeAll<T>(this IStorableSpawnedInstanceReceiver<T> self, Action<T> callback)
        {
            self.InstanceList.ToList().ForEach(callback);
        }

        public static IEnumerable<TResult> InvokeAll<T, TResult>(this IStorableSpawnedInstanceReceiver<T> self, Func<T, TResult> callback)
        {
            return self.InstanceList.ToList().Select(callback);
        }
    }
}
