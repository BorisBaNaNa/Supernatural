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

        public void SetAnimation(Animation animation, bool loop, bool AddToCurrent = false)
        {
            TrackEntry currentAnim = Skeleton.AnimationState.GetCurrent(_curTrackId);

            if (AddToCurrent || currentAnim != null && !currentAnim.Loop)
                Skeleton.AnimationState.AddAnimation(_curTrackId, animation, loop, 0);
            else
                Skeleton.AnimationState.SetAnimation(_curTrackId, animation, loop);
        }
    }
}
