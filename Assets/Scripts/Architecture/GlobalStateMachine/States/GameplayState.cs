using Architecture.Services;
using UI.Factory;

namespace Architecture.GlobalStateMachine.States
{
    public class GameplayState : IState
    {
        private readonly SceneLoader _sceneLoader;
        private readonly UIFactory _uiFactory;

        public GameplayState(SceneLoader sceneLoader, UIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            _sceneLoader.LoadScene(Scenes.GameplayScene);
        }

        public void Exit()
        {
        }
    }
}