using Assets.Supernatural.Scripts.AnimStateMachine;
using Assets.Supernatural.Scripts.AnimStateMachine.BaseStates;
using Assets.Supernatural.Scripts.Player.Controllers;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public abstract class MovementStateBase : AnimationStateBase
    {
        protected SpineStateMachine SpineSwitcher => _switcher as SpineStateMachine;

        protected PlayerMovementAnimationsReferences _animationsReferences;

        public MovementStateBase(PlayerMovementAnimationsReferences animationsReferences) => _animationsReferences = animationsReferences;
    }
}