using Architecture.Boot;
using Architecture.GlobalStateMachine;
using Architecture.GlobalStateMachine.Factory;
using Architecture.GlobalStateMachine.States;
using Architecture.Services;
using UI.Factory;
using Zenject;

namespace Installers
{
    public class Bootstrapper : MonoInstaller, ICoroutineRunner, IInitializable
    {
        public override void InstallBindings()
        {
            BindInstaller();
            BindAssetProvider();
            BindUIFactory();
            BindGameStateMachine();
            BindStateFactory();
            BindGameStates();
            BindSceneLoader();
        }

        private void BindInstaller() =>
            Container.BindInterfacesTo<Bootstrapper>().FromInstance(this).AsSingle();

        private void BindAssetProvider()
        {
            Container.BindInterfacesAndSelfTo<AssetProvider>().AsSingle();
        }

        private void BindUIFactory()
        {
            Container.BindInterfacesAndSelfTo<UIFactory>().AsSingle();
        }

        private void BindGameStateMachine() =>
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();

        private void BindStateFactory() =>
            Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();

        private void BindGameStates()
        {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<MainMenuState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplayState>().AsSingle();
        }

        private void BindSceneLoader() =>
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();

        public void Initialize() =>
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
    }
}