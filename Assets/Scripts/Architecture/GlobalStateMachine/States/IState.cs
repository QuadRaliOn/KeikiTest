namespace Architecture.GlobalStateMachine.States{
	public interface IState{
		public void Enter();

		public void Exit();
	}

	public interface IPayloadState<TPayload> : IState
	{
		public void Enter(TPayload payload);
	}
}