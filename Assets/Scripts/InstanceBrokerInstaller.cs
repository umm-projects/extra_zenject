using Zenject;

namespace ExtraZenject
{
    public class InstanceBrokerInstaller : MonoInstaller<InstanceBrokerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<InstanceBroker>().AsSingle();
        }
    }
}