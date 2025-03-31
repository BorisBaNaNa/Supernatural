using Assets.Supernatural.Scripts.Player.AnimationStates.Once;
using System;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Abilities
{
    [Serializable]
    public class SamPersonAbility : UltimateAbilityBase
    {
        public override void Activate(PlayerController playerContext)
        {
            base.Activate(playerContext);
            playerContext.AnimStateMachine.TryGetState<UltimateState>(out var state);
            state.SetLoop(false);
            playerContext.AnimStateMachine.StateSwitch<UltimateState>();
            state.AddOnCompleteListener((_) => Deactivate(playerContext));

            playerContext.DisableMovement();
            playerContext.DisableAttack();
        }

        public override void Deactivate(PlayerController playerContext)
        {
            base.Deactivate(playerContext);
            playerContext.AnimStateMachine.DropCurrentState();

            playerContext.EnableMovement();
            playerContext.EnableAttack();
        }

        public override void UpdateAbility(PlayerController playerContext)
        {
        }

        public override bool HandleDamageTaken(PlayerController playerContext, float damage, Vector2 force, GameObject instigator)
        {
            Deactivate(playerContext);
            return false;
        }
    }
}
