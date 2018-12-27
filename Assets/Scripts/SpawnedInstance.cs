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
}