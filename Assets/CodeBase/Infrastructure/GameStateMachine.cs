using System;
using System.Collections.Generic;

namespace CodeBase.Infrastructure
{
    public class GameStateMachine
    {
        private readonly Dictionary<Type, IState> _states;
        private IState _activateState;

        public GameStateMachine() {
            _states = new Dictionary<Type, IState>() {
                [typeof(BootstrapState)] = new BootstrapState(this),
            };
        }
        
        public void Enter<TState>() where TState : IState {
            _activateState?.Exit();
            var state = _states[typeof(TState)];
            _activateState = state; 
            state.Enter();
        }
    }
}