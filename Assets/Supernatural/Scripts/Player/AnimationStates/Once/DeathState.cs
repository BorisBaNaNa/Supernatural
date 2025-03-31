using Assets.Supernatural.Scripts.Player.AnimationStates.Movement;
using Assets.Supernatural.Scripts.Player.Configs;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Once
{
    public class DeathState : SpineAnimationStateBase<PlayerMainAnimationsReferences>
    {
        public DeathState(PlayerMainAnimationsReferences animationsReferences) : base(animationsReferences)
        {
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Death, false, true);
        }

        public override void Exit()
        {
        }
    }
}
