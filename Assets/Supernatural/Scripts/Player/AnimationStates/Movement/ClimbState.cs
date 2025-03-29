using Assets.Supernatural.Scripts.Player.Controllers;
using UnityEngine;
using static Assets.Supernatural.Scripts.Player.Controllers.PlayerMovementController;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public class ClimbState : MovementStateBase
    {
        private CachedMovementData _cachedMovement;
        private float _lastVelocityYDirection;

        private const float MIN_VELOCITY = 0.05f;

        public ClimbState(PlayerMovementAnimationsReferences animationsReferences,
            CachedMovementData cachedData) : base(animationsReferences)
        {
            _cachedMovement = cachedData;
        }

        public override void Enter()
        {
            _lastVelocityYDirection = float.MaxValue;
            HandleClimbingStates();
        }

        public override void Exit()
        {
            //SpineSwitcher.SetReverseToCurrentAnimation(false);
        }

        public override void Update()
        {
            if (_cachedMovement.IsClimbingOnWall)
                HandleClimbingStates();
            else
            {
                if (WalkState.ItsMe(_cachedMovement))
                    _switcher.StateSwitch<WalkState>();
                else if (IdleState.ItsMe(_cachedMovement))
                    _switcher.StateSwitch<IdleState>();
            }
        }

        public static bool ItsMe(CachedMovementData cachedData)
            => cachedData.IsClimbingOnWall;

        private void HandleClimbingStates()
        {
            float velocityYDirection =
                _cachedMovement.Velocity.y > MIN_VELOCITY ? 1f
                : _cachedMovement.Velocity.y < -MIN_VELOCITY ? -1f : 0f;

            if (_lastVelocityYDirection == velocityYDirection)
                return;
            if (velocityYDirection == 0)
                SpineSwitcher.SetAnimation(_animationsReferences.ClimbIdle, true, true);
            else
            {
                if (_lastVelocityYDirection == 0 || _lastVelocityYDirection == float.MaxValue)
                    SpineSwitcher.SetAnimation(_animationsReferences.Climb, true, true);

                SpineSwitcher.SetReverseToCurrentAnimation(velocityYDirection < 0);
            }

            _lastVelocityYDirection = velocityYDirection;
        }
    }
}