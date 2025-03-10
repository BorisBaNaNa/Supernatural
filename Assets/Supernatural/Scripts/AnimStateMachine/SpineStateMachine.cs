using Spine;
using Spine.Unity;

namespace Assets.Supernatural.Scripts.AnimStateMachine
{
    public class SpineStateMachine : AnimationStateMachine
    {
        public SkeletonAnimation Skeleton { get; private set; }

        private int _curTrackId;

        public SpineStateMachine(SkeletonAnimation skeleton, int currentTrackIndex)
        {
            Skeleton = skeleton;
            _curTrackId = currentTrackIndex;
        }

        private void SetAnimation(string animationName, bool loop, bool hardSet = true)
        {
            if (hardSet)
                Skeleton.AnimationState.SetAnimation(_curTrackId, animationName, loop);
            else
                Skeleton.AnimationState.AddAnimation(_curTrackId, animationName, loop, 0);
        }
    }
}
