using Spine;
using Spine.Unity;
using System;
using UnityEngine;

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

        public override void DropCurrentState()
        {
            base.DropCurrentState();
            Skeleton.AnimationState.SetEmptyAnimation(_curTrackId, 0f);
        }

        public void SetAnimation(Spine.Animation animation, bool loop, bool hardSet = false)
        {
            if (!hardSet && TryGetCurrentTrack(out var currentAnimTrack) && !currentAnimTrack.Loop)
                Skeleton.AnimationState.AddAnimation(_curTrackId, animation, loop, 0);
            else
                Skeleton.AnimationState.SetAnimation(_curTrackId, animation, loop);
        }

        public void SetEmptyAnimation() => Skeleton.AnimationState.SetEmptyAnimation(_curTrackId, 0.1f);

        public void SetReverseToCurrentAnimation(bool reverse)
        {
            if (!TryGetCurrentTrack(out var currentAnimTrack))
                return;
            if (reverse)
                currentAnimTrack.End += EndReverceAnimationEvent;

            currentAnimTrack.Reverse = reverse;
        }

        public bool TryGetCurrentTrack(out TrackEntry currentAnimTrack)
        {
            currentAnimTrack = Skeleton.AnimationState.GetCurrent(_curTrackId);
            return currentAnimTrack != null;
        }

        private void EndReverceAnimationEvent(TrackEntry track)
        {
            track.Reverse = false;
            track.End -= EndReverceAnimationEvent;
        }
    }
}
