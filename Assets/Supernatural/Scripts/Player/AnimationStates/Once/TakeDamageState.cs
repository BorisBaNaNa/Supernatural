using Assets.Supernatural.Scripts.Player.AnimationStates.Movement;
using Assets.Supernatural.Scripts.Player.Configs;
using Spine;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Once
{
    public class TakeDamageState : SpineAnimationStateBase<PlayerMainAnimationsReferences>
    {
        public TakeDamageState(PlayerMainAnimationsReferences animationsReferences) : base(animationsReferences)
        {
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.TakeDamage, false, true);

            if (!SpineSwitcher.TryGetCurrentTrack(out var track))
            {
                Debug.LogError("Can't find current track");
                return;
            }

            track.Complete += OnAnimationEnd;
        }

        public override void Exit()
        {
        }


        private void OnAnimationEnd(TrackEntry trackEntry)
        {
            SpineSwitcher.DropCurrentState();
            trackEntry.End -= OnAnimationEnd;
        }
    }
}
