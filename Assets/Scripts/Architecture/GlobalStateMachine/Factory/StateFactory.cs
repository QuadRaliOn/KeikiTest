using Architecture.GlobalStateMachine.States;
using Zenject;

namespace Architecture.GlobalStateMachine.Factory{
	public class StateFactory : IStateFactory{
		private readonly DiContainer _container;

		public StateFactory(DiContainer container){
			_container = container;
		}

		public T GetState<T>() where T : class, IState{
			return _container.Resolve<T>();
		}
	}
}