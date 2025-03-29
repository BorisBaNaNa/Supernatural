using Assets.Supernatural.Scripts.Player.Controllers;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public class JumpState : MovementStateBase
    {
        private PlayerMovementController.CachedMovementData _cachedMovement;

        public JumpState(PlayerMovementAnimationsReferences animationsReferences,
            PlayerMovementController.CachedMovementData cachedData) : base(animationsReferences)
        {
            _cachedMovement = cachedData;
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Jump, false);
        }

        public override void Exit()
        {
        }


        public override void Update()
        {
            if (ClimbState.ItsMe(_cachedMovement))
                _switcher.StateSwitch<ClimbState>();
            else if (Mathf.Abs(_cachedMovement.Velocity.y) == 0)
            {
                if (WalkState.ItsMe(_cachedMovement))
                    _switcher.StateSwitch<WalkState>();
                else if (IdleState.ItsMe(_cachedMovement))
                    _switcher.StateSwitch<IdleState>();
            }
        }
    }
}