using Assets.Supernatural.Scripts.Interfaces;
using System;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Abilities
{
    [Serializable]
    public abstract class UltimateAbilityBase : IUltimateAbility
    {
        public bool IsActive => _isActive;

        protected bool _isActive;

        public virtual void Activate(PlayerController playerContext) => _isActive = true;

        public virtual void Deactivate(PlayerController playerContext) => _isActive = false;

        public abstract bool HandleDamageTaken(PlayerController playerContext, float damage, Vector2 force, GameObject instigator);
        public abstract void UpdateAbility(PlayerController playerContext);
    }
}
