namespace Architecture.GlobalStateMachine.States
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine _stateMachine;

        public BootstrapState(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            // QualitySettings.vSyncCount = 0;
            // Application.targetFrameRate = 60;

            _stateMachine.Enter<GameplayState>();
        }

        public void Exit()
        {
        }
    }
}