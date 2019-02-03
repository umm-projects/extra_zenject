using Zenject;

namespace ExtraZenject
{
    public interface IInstancePublisher
    {
    }

    public static class InstancePublishableExtension
    {
        public static void Publish<T>(this IInstancePublisher self)
        {
            self.Publish((T) self);
        }

        public static void Publish<T>(this IInstancePublisher self, T instance)
        {
            ProjectContext
                .Instance
                .Container
                .Resolve<IInstanceBroker>()
                .Publish(instance);
        }
    }
}