using Assets.Supernatural.Scripts.AnimStateMachine.Interfces;
using System;
using System.Collections.Generic;

namespace Assets.Supernatural.Scripts.AnimStateMachine
{
    public class AnimationStateMachine : IAnimationStateSwitcher
    {
        private readonly Dictionary<Type, IAnimationState> _states = new();

        private IAnimationState _currentState;

        public void AddState<TState>(TState state) where TState : IAnimationState
        {
            _states[typeof(TState)] = state;
            state.Init(this);
        }

        public void StateSwitch<TState>() where TState : IAnimationState
        {
            if (!_states.TryGetValue(typeof(TState), out var newState))
                throw new KeyNotFoundException($"State {typeof(TState)} not registered!");

            //if (newState == _currentState)
            //    return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public bool TryGetState<TState>(out TState state) where TState : IAnimationState
        {
            var stateIsFinded = _states.TryGetValue(typeof(TState), out var gettingState);
            state = stateIsFinded ? (TState)gettingState : default;
            return stateIsFinded;
        }

        public virtual void DropCurrentState()
        {
            _currentState?.Exit();
            _currentState = null;
        }

        public void Update()
        {
            _currentState?.Update();
        }
    }
}
