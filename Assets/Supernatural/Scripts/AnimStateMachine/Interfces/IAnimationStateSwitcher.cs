using Assets.Supernatural.Scripts.AnimStateMachine.BaseStates;
using System;

namespace Assets.Supernatural.Scripts.AnimStateMachine.Interfces
{
    public interface IAnimationStateSwitcher
    {
        void StateSwitch<TState>() where TState : IAnimationState;
        void StateSwitchTemporary<TState>(Type[] nextStates = null) where TState : AnimationTempStateBase;
        void GoToNextState();
    }
}
