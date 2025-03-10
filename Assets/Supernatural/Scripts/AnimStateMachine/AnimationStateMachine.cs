using Assets.Supernatural.Scripts.AnimStateMachine.BaseStates;
using Assets.Supernatural.Scripts.AnimStateMachine.Interfces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Supernatural.Scripts.AnimStateMachine
{
    public class AnimationStateMachine : IAnimationStateSwitcher
    {
        private readonly Dictionary<Type, IAnimationState> _states = new();
        private readonly Queue<IAnimationState> _statesQueue = new();

        private IAnimationState _currentState;

        public void AddState<TState>(IAnimationState state) where TState : IAnimationState
        {
            _states[typeof(TState)] = state;
            state.Init(this);
        }

        public void StateSwitch<TState>() where TState : IAnimationState
        {
            if (!_states.TryGetValue(typeof(TState), out var newState))
                throw new KeyNotFoundException($"State {typeof(TState)} not registered!");

            if (_statesQueue.Count > 0)
                _statesQueue.Clear();

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void StateSwitchTemporary<TState>(Type[] nextStates = null) where TState : AnimationTempStateBase
        {
            InitStatesQueue(nextStates);

            if (!_states.TryGetValue(typeof(TState), out var newState))
                throw new KeyNotFoundException($"Temporary state {typeof(TState)} not registered!");

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void GoToNextState()
        {
            if (_statesQueue.Count > 0)
            {
                _currentState?.Exit();
                _currentState = _statesQueue.Dequeue();
                _currentState.Enter();
            }
            else
            {
                // Можно предусмотреть логику, если очередь пуста (например, вернуться в состояние по умолчанию)
                Debug.Log("State queue is empty. No state to revert to.");
                _currentState = null;
            }
        }

        public void DropCurrentState()
        {
            _currentState?.Exit();
            _currentState = null;
        }

        public void Update()
        {
            _currentState?.Update();
        }

        private void InitStatesQueue(Type[] nextStates)
        {
            if (nextStates != null)
            {
                if (!nextStates.All(state => _states.ContainsKey(state)))
                    throw new KeyNotFoundException($"States {string.Join<Type>(", ", nextStates)} not registered!");

                foreach (var state in nextStates)
                    _statesQueue.Enqueue(_states[state]);
            }
        }
    }
}
