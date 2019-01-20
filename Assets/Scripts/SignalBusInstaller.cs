using Zenject;

namespace ExtraZenject
{
    public class SignalBusInstaller : MonoInstaller<SignalBusInstaller>
    {
        public override void InstallBindings()
        {
            Zenject.SignalBusInstaller.Install(Container);
        }
    }
}