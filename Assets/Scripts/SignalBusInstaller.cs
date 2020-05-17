using Zenject;

namespace ExtraZenject
{
    public class SignalBusInstaller : MonoInstaller<SignalBusInstaller>
    {
        public override void InstallBindings()
        {
            if (!Container.HasBinding<SignalBus>())
            {
                Zenject.SignalBusInstaller.Install(Container);
            }
        }
    }
}
