using Assets.Supernatural.Scripts.Player;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Interfaces
{
    public interface IUltimateAbility
    {
        bool IsActive { get; }

        void Activate(PlayerController playerContext);

        void UpdateAbility(PlayerController playerContext);

        void Deactivate(PlayerController playerContext);

        bool HandleDamageTaken(PlayerController playerContext, float damage, Vector2 force, GameObject instigator);
    }
}
