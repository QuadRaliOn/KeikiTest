using System;
using Architecture.Boot;
using Architecture.GlobalStateMachine;
using Architecture.GlobalStateMachine.Factory;
using Architecture.GlobalStateMachine.States;
using Architecture.Services;
using GamePlay;
using GamePlay.Factory;
using GamePlay.Tracing;
using UI.Factory;
using UnityEngine;
using Zenject;

namespace Installers {
    public class Bootstrapper : MonoInstaller, ICoroutineRunner, IInitializable {
        public override void InstallBindings() {
            BindInstaller();
            BindAssetProvider();
            BindUIFactory();
            BindGameplayFactory();
            BindGameStateMachine();
            BindStateFactory();
            BindGameStates();
            BindSceneLoader();
            BindLevelService();
            BindSoundService();
            BindIdleHintService();
        }

        private void BindInstaller() =>
            Container.BindInterfacesTo<Bootstrapper>().FromInstance(this).AsSingle();

        private void BindAssetProvider() => 
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();

        private void BindUIFactory() => 
            Container.Bind<IUIFactory>().To<UIFactory>().AsSingle();

        private void BindGameplayFactory() => 
            Container.Bind<IGameplayFactory>().To<GameplayFactory>().AsSingle();

        private void BindGameStateMachine() =>
            Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();

        private void BindStateFactory() =>
            Container.Bind<IStateFactory>().To<StateFactory>().AsSingle();

        private void BindGameStates() {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<MainMenuState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplayState>().AsSingle();
            Container.BindFactory<GamePlayPanel, LevelData, GameplaySession, GameplaySession.Factory>()
                .FromSubContainerResolve()
                .ByMethod(InstallGameplaySession);
        }

        private void InstallGameplaySession(DiContainer subContainer, GamePlayPanel panel, LevelData level) {
            subContainer.BindInstance(panel);
            subContainer.BindInstance(level);
            subContainer.Bind<MascotPresenter>().AsSingle();
            subContainer.Bind<TracingTrailRenderer>().AsSingle();
            subContainer.Bind<TracingPointsRenderer>().AsSingle();
            subContainer.Bind<TracingStrokeView>().AsSingle();
            subContainer.Bind<TracingInputProcessor>().AsSingle();
            subContainer.Bind<GameplaySession>().AsSingle();
        }

        private void BindSceneLoader() {
            Container.Bind<ISceneTransitionService>().To<SceneTransitionService>().AsSingle();
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
        }

        private void BindLevelService() {
            LevelDatabase db = UnityEngine.Resources.Load<LevelDatabase>("LevelDatabase");
            if (db == null) {
                db = UnityEngine.ScriptableObject.CreateInstance<LevelDatabase>();
                UnityEngine.Debug.LogWarning("LevelDatabase asset not found in Resources. Using empty instance.");
            }
            Container.Bind<LevelDatabase>().FromInstance(db).AsSingle();
            Container.Bind<ILevelService>().To<LevelService>().AsSingle();
        }

        private void BindSoundService() =>
            Container.Bind<ISoundService>().To<SoundService>().AsSingle();

        private void BindIdleHintService() =>
            Container.Bind<IIdleHintService>().To<IdleHintService>().AsSingle();

        public void Initialize() =>
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
    }
}