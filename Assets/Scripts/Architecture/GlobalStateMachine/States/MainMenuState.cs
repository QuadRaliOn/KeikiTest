using Architecture.Services;
using UI.Factory;
using UnityEngine.SceneManagement;

namespace Architecture.GlobalStateMachine.States {
    public class MainMenuState : IState {
        private readonly SceneLoader _sceneLoader;
        private readonly UIFactory _uiFactory;

        public MainMenuState(SceneLoader sceneLoader, UIFactory uiFactory) {
            _uiFactory = uiFactory;
            _sceneLoader = sceneLoader;
        }

        public void Enter() {
            _sceneLoader.LoadScene(Scenes.MainMenuScene,OnSceneLoaded);
        }

        private void OnSceneLoaded() {
            _uiFactory.CreateMainMenuPanel();
        }

        public void Exit() {
        }
    }
}