using Architecture.GlobalStateMachine.States;

namespace Architecture.GlobalStateMachine{
	public interface IGameStateMachine{
		void Enter<TState>() where TState : class, IState;
		void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>;
	}
}