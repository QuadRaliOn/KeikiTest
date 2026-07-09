using System;
using Architecture.Boot;
using Architecture.GlobalStateMachine;
using Architecture.GlobalStateMachine.Factory;
using Architecture.GlobalStateMachine.States;
using Architecture.Services;
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
        }

        private void BindInstaller() =>
            Container.BindInterfacesTo<Bootstrapper>().FromInstance(this).AsSingle();

        private void BindAssetProvider() => 
            Container.BindInterfacesAndSelfTo<AssetProvider>().AsSingle();

        private void BindUIFactory() => 
            Container.BindInterfacesAndSelfTo<UIFactory>().AsSingle();

        private void BindGameplayFactory() => 
            Container.BindInterfacesAndSelfTo<GameplayFactory>().AsSingle();

        private void BindGameStateMachine() =>
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();

        private void BindStateFactory() =>
            Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();

        private void BindGameStates() {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<MainMenuState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplayState>().AsSingle();
            Container.BindFactory<GamePlayPanel, LevelData, GameplaySession, GameplaySession.Factory>();
        }

        private void BindSceneLoader() =>
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();

        private void BindLevelService() {
            LevelDatabase db = UnityEngine.Resources.Load<LevelDatabase>("LevelDatabase");
            if (db == null) {
                db = UnityEngine.ScriptableObject.CreateInstance<LevelDatabase>();
                UnityEngine.Debug.LogWarning("LevelDatabase asset not found in Resources. Using empty instance.");
            }
            Container.Bind<LevelDatabase>().FromInstance(db).AsSingle();
            Container.Bind<LevelService>().AsSingle();
        }

        private void BindSoundService() =>
            Container.BindInterfacesAndSelfTo<SoundService>().AsSingle();

        public void Initialize() =>
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
    }
}