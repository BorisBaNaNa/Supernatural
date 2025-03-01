using System;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player
{
    public interface IPlayerInputController : Assets.Supernatural.Scripts.Infrastructure.IService
    {
        event Action OnJumpPerformed;
        event Action OnJumpOffPerformed;
        event Action OnRangeAttackPerformed;
        event Action OnMeleeAttackPerformed;

        void Initialize();

        Vector2 ReadMovementInput();

        void Enable();
        void Disable();

        void EnableMovement();
        void DisableMovement();
        void EnableAttack();
        void DisableAttack();
    }
}
