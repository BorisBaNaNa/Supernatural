using Assets.Supernatural.Scripts.AnimStateMachine;
using Assets.Supernatural.Scripts.AnimStateMachine.BaseStates;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public abstract class SpineAnimationStateBase<AnimRefConfigT> : AnimationStateBase where AnimRefConfigT : ScriptableObject
    {
        protected SpineStateMachine SpineSwitcher => _switcher as SpineStateMachine;

        protected AnimRefConfigT _animationsReferences;

        public SpineAnimationStateBase(AnimRefConfigT animationsReferences) => _animationsReferences = animationsReferences;
    }
}