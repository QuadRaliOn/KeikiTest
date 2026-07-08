using Architecture.GlobalStateMachine.Factory;
using Architecture.GlobalStateMachine.States;

namespace Architecture.GlobalStateMachine{
	public class GameStateMachine : IGameStateMachine{
		private IState _activeState;
		private readonly IStateFactory _stateFactory;

		public GameStateMachine(IStateFactory stateFactory){
			_stateFactory = stateFactory;
		}

		public void Enter<TState>() where TState : class, IState{
			var state = _stateFactory.GetState<TState>();
			_activeState?.Exit();
			_activeState = state;

			state.Enter();
		}

		public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>{
			var state = _stateFactory.GetState<TState>();
			_activeState?.Exit();
			_activeState = state;

			state.Enter(payload);
		}
	}
}