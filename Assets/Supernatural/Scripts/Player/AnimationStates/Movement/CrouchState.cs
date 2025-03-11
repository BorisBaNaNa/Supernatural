using Assets.Supernatural.Scripts.Player.Controllers;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public class CrouchState : MovementStateBase
    {
        public CrouchState(PlayerMovementAnimationsReferences animationsReferences) : base(animationsReferences)
        {
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Crouch, false);
            SpineSwitcher.SetAnimation(_animationsReferences.Crouching, true, true);
        }

        public override void Exit()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.UnCrouch, false);
        }
    }
}