using Assets.Supernatural.Scripts.Player.Controllers;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public class WalkState : MovementStateBase
    {
        public WalkState(PlayerMovementAnimationsReferences animationsReferences) : base(animationsReferences)
        {
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Walk, true);
        }

        public override void Exit()
        {
        }
    }
}