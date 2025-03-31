using Assets.Supernatural.Scripts.Player.AnimationStates.Movement;
using Assets.Supernatural.Scripts.Player.Configs;
using Spine;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Once
{
    public class UltimateState : SpineAnimationStateBase<PlayerMainAnimationsReferences>
    {
        private bool _loop = true;

        public UltimateState(PlayerMainAnimationsReferences animationsReferences) : base(animationsReferences)
        {
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.Ultimate, _loop, true);
        }

        public override void Exit()
        {
            SpineSwitcher.SetEmptyAnimation();
        }

        public void SetLoop(bool loop) => _loop = loop;

        public void AddOnCompleteListener(AnimationState.TrackEntryDelegate onComplete)
        {
            SpineSwitcher.TryGetCurrentTrack(out var trackEntry);

            if (trackEntry != null)
                trackEntry.Complete += onComplete;
        }
    }
}
