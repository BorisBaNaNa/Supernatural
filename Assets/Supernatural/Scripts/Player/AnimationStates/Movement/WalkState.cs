using Assets.Supernatural.Scripts.Player.Configs;
using UnityEditorInternal;
using UnityEngine;
using static Assets.Supernatural.Scripts.Player.Controllers.PlayerMovementController;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public class WalkState : SpineAnimationStateBase<PlayerMovementAnimationsReferences>
    {
        private readonly CachedMovementData _cachedMovement;

        public WalkState(PlayerMovementAnimationsReferences animationsReferences, CachedMovementData cachedData) : base(animationsReferences)
        {
            _cachedMovement = cachedData;
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Walk, true);
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            if (ClimbState.ItsMe(_cachedMovement))
                _switcher.StateSwitch<ClimbState>();
            else if (IdleState.ItsMe(_cachedMovement))
                _switcher.StateSwitch<IdleState>();
        }

        public static bool ItsMe(CachedMovementData cachedData)
            => Mathf.Abs(cachedData.Velocity.x) >= 1f && cachedData.InputDir.x != 0;
    }
}