using Zenject;

namespace ExtraZenject
{
    public interface ISpawnedInstancePublisher
    {
        // ReSharper disable once UnusedMemberInSuper.Global
        SignalBus Publisher { get; set; }
    }

    public static class SpawnedInstancePublisherExtension
    {
        public static void OnSpawn<T>(this ISpawnedInstancePublisher self)
        {
            self.Publisher.TryFire(new SpawnedInstance<T>((T) self));
        }
    }
}