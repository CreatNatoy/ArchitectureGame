using System;
using System.Collections.Generic;

namespace CodeBase.Infrastructure
{
    public class GameStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _activateState;

        public GameStateMachine(SceneLoader sceneLoader) {
            _states = new Dictionary<Type, IExitableState>() {
                [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader),
                [typeof(LoadLevelState)] = new LoadLevelState(this, sceneLoader),

            };
        }

        public void Enter<TState>() where TState : class, IState {
            var state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload> {
            var state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState {
            _activateState?.Exit();
            var state = GetState<TState>();
            _activateState = state;
            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState => _states[typeof(TState)] as TState;
    }
}