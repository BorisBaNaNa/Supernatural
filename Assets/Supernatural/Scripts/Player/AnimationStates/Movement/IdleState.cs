using Assets.Supernatural.Scripts.Player.Controllers;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Movement
{
    public class IdleState : MovementStateBase
    {
        public IdleState(PlayerMovementAnimationsReferences animationsReferences) : base(animationsReferences)
        {
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Idle, true);
        }

        public override void Exit()
        {
        }
    }
}