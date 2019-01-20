using Zenject;

namespace ExtraZenject
{
    public struct SpawnedInstance<T>
    {
        public T Instance { get; }

        public SpawnedInstance(T instance)
        {
            Instance = instance;
        }
    }

    public static class SpawnedInstanceExtension
    {
        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder DeclareSpawnedSignal<TSignal>(this DiContainer container)
        {
            return container.DeclareSignal<SpawnedInstance<TSignal>>();
        }
    }
}