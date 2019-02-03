using UniRx;
using Zenject;

namespace ExtraZenject
{
    public class MessageBrokerInstaller : MonoInstaller<MessageBrokerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MessageBroker>().AsCached();
        }
    }
}