using Assets.Supernatural.Scripts.Player.Controllers;
using UnityEditorInternal;
using UnityEngine;
using static Assets.Supernatural.Scripts.Player.Controllers.PlayerMovementController;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public class IdleState : MovementStateBase
    {
        private CachedMovementData _cachedMovement;

        public IdleState(PlayerMovementAnimationsReferences animationsReferences,
            CachedMovementData cachedData) : base(animationsReferences)
        {
            _cachedMovement = cachedData;
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Idle, true);
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            if (ClimbState.ItsMe(_cachedMovement))
                _switcher.StateSwitch<ClimbState>();
            if (WalkState.ItsMe(_cachedMovement))
                _switcher.StateSwitch<WalkState>();
        }

        public static bool ItsMe(CachedMovementData cachedMovement) 
            => Mathf.Abs(cachedMovement.Velocity.x) < 1f && cachedMovement.InputDir.x == 0;
    }
}