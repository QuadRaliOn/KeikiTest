using Architecture.Services;
using UI.Factory;

namespace Architecture.GlobalStateMachine.States
{
    public class MainMenuState : IState
    {
        private readonly SceneLoader _sceneLoader;
        private readonly UIFactory _uiFactory;

        public MainMenuState(SceneLoader sceneLoader, UIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _sceneLoader.LoadScene(Scenes.MainMenuScene);
        }

        public void Exit()
        {
        }
    }
}