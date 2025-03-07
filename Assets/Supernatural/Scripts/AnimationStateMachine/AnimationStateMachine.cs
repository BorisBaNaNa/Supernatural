using System.Linq;
using UnityEngine;

namespace Supernatural.Scripts.AnimationStateMachine
{
    public class AnimationStateMachine : IAnimationStateSwitcher
    {
        private readonly IAnimationState[] _states;

        private IAnimationState _currentState;

        public AnimationStateMachine(IAnimationState[] states)
        {
            _states = states;
        }

        public void StateSwitch<TState>() where TState : IAnimationState
        {
            _currentState?.Exit();
            _currentState = _states.FirstOrDefault(x => x is TState);
            _currentState.Enter();
        }
    }

    public interface IAnimationState
    {
        void Enter();
        void Exit();
        void Update();
    }

    public interface IAnimationStateSwitcher
    {
        void StateSwitch<TState>() where TState : IAnimationState;
    }
}
