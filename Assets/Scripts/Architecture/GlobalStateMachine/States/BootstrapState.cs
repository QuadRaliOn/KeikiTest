using UnityEngine;

namespace Architecture.GlobalStateMachine.States
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine _stateMachine;

        public BootstrapState(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter() {
            SetLandscapeOrientation();

            _stateMachine.Enter<MainMenuState>();
        }

        public void Exit()
        {
        }

        private static void SetLandscapeOrientation() {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }
}