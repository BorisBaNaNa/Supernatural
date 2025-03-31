using Assets.Supernatural.Scripts.Player.AnimationStates.Movement;
using Assets.Supernatural.Scripts.Player.Configs;
using Spine;
using System;

namespace Assets.Supernatural.Scripts.Player.AnimationStates.Once
{
    public class RangeAttackState : SpineAnimationStateBase<PlayerAttackAnimationsReferences>
    {
        private Action _attackAction;

        private const string ATTACK_EVENT_NAME = "Attack";

        public RangeAttackState(PlayerAttackAnimationsReferences animationsReferences, Action attackAction) : base(animationsReferences)
        {
            _attackAction = attackAction;
        }

        public override void Enter()
        {
            SpineSwitcher.SetAnimation(_animationsReferences.RangeAttackAnim, false, true);

            SpineSwitcher.TryGetCurrentTrack(out var trackEntry);
            trackEntry.Complete += _ => SpineSwitcher.DropCurrentState();
            trackEntry.Event += CheckAttcackEvent;
        }

        public override void Exit()
        {
            SpineSwitcher.SetEmptyAnimation();
        }

        private void CheckAttcackEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == ATTACK_EVENT_NAME)
                _attackAction?.Invoke();
        }
    }

}
