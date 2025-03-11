using Assets.Supernatural.Scripts.Player.Controllers;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public class JumpState : MovementStateBase
    {
        public JumpState(PlayerMovementAnimationsReferences animationsReferences) : base(animationsReferences)
        {
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Jump, false);
        }

        public override void Exit()
        {
        }
    }
}