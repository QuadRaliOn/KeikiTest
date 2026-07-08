using Architecture.GlobalStateMachine.States;

namespace Architecture.GlobalStateMachine.Factory{
	public interface IStateFactory
	{
		T GetState<T>() where T : class, IState;
	}
}