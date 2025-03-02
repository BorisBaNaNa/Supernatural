using Assets.Supernatural.Scripts.Interfaces;
using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Supernatural.Scripts.Input
{
    public class KeyBoardPlayerController : IPlayerInputController
    {
        public event Action OnJumpPerformed;
        public event Action OnJumpOffPerformed;
        public event Action OnRangeAttackPerformed;
        public event Action OnMeleeAttackPerformed;

        private InputActions _inputs;

        public KeyBoardPlayerController()
        {
            _inputs = new InputActions();
        }

        public void Initialize()
        {
            _inputs.Player.Jump.performed += Jump;
            _inputs.Player.Jump.canceled += JumpOff;
            _inputs.Player.RangeAttack.performed += RangeAttack;
            _inputs.Player.MeleeAttack.performed += MeleeAttack;
        }

        public void Dispose() => _inputs.Dispose();

        public void Enable() => _inputs.Player.Enable();

        public void Disable() => _inputs.Player.Disable();

        public void EnableAttack()
        {
            _inputs.Player.RangeAttack.Enable();
            _inputs.Player.MeleeAttack.Enable();
        }

        public void DisableAttack()
        {
            _inputs.Player.RangeAttack.Disable();
            _inputs.Player.MeleeAttack.Disable();
        }

        public void EnableMovement() => _inputs.Player.Move.Enable();

        public void DisableMovement() => _inputs.Player.Move.Disable();

        public Vector2 ReadMovementInput() => _inputs.Player.Move.ReadValue<Vector2>();

        private void Jump(CallbackContext _) => OnJumpPerformed?.Invoke();

        private void JumpOff(CallbackContext _) => OnJumpOffPerformed?.Invoke();

        private void RangeAttack(CallbackContext _) => OnRangeAttackPerformed?.Invoke();

        private void MeleeAttack(CallbackContext _) => OnMeleeAttackPerformed?.Invoke();
    }
}
