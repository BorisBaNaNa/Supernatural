using Assets.Supernatural.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Supernatural.Scripts.Input
{
    public class UIPlayerController : MonoBehaviour, IPlayerInputController
    {
        public event Action OnJumpPerformed;
        public event Action OnJumpOffPerformed;
        public event Action OnRangeAttackPerformed;
        public event Action OnMeleeAttackPerformed;

        [SerializeField] private Button _rangeAttackBtn;
        [SerializeField] private Button _meleeAttackBtn;
        [SerializeField] private EventTrigger _jumpBtn;
        [SerializeField] private EventTrigger _moveLeftBtn;
        [SerializeField] private EventTrigger _moveRightBtn;
        [SerializeField] private EventTrigger _moveDownBtn;

        private Vector2 _moveDir;

        public void Initialize()
        {
            _rangeAttackBtn.onClick.AddListener(RangeAttack);
            _meleeAttackBtn.onClick.AddListener(MeleeAttack);

            AddTriggerEvent(_jumpBtn, EventTriggerType.PointerDown, Jump);
            AddTriggerEvent(_jumpBtn, EventTriggerType.PointerUp, JumpOff);
            AddTriggerEvent(_moveLeftBtn, EventTriggerType.PointerDown, MoveLeftStart);
            AddTriggerEvent(_moveLeftBtn, EventTriggerType.PointerUp, MoveXStop);
            AddTriggerEvent(_moveRightBtn, EventTriggerType.PointerDown, MoveRightStart);
            AddTriggerEvent(_moveRightBtn, EventTriggerType.PointerUp, MoveXStop);
            AddTriggerEvent(_moveDownBtn, EventTriggerType.PointerDown, MoveDownStart);
            AddTriggerEvent(_moveDownBtn, EventTriggerType.PointerUp, MoveYStop);
        }

        public void Dispose()
        {
            _rangeAttackBtn.onClick.RemoveListener(RangeAttack);
            _meleeAttackBtn.onClick.RemoveListener(MeleeAttack);

            _jumpBtn.triggers.Clear();
            _moveLeftBtn.triggers.Clear();
            _moveRightBtn.triggers.Clear();
            _moveDownBtn.triggers.Clear();
        }

        public Vector2 ReadMovementInput() => _moveDir;

        public void Enable() => gameObject?.SetActive(true);

        public void Disable() => gameObject?.SetActive(false);

        public void EnableMovement()
        {
            _moveLeftBtn.enabled = true;
            _moveRightBtn.enabled = true;
            _moveDownBtn.enabled = true;
        }

        public void DisableMovement()
        {
            _moveLeftBtn.enabled = false;
            _moveRightBtn.enabled = false;
            _moveDownBtn.enabled = false;
        }

        public void EnableAttack()
        {
            _meleeAttackBtn.interactable = true;
            _rangeAttackBtn.interactable = true;
        }

        public void DisableAttack()
        {
            _meleeAttackBtn.interactable = false;
            _rangeAttackBtn.interactable = false;
        }

        private void MoveLeftStart() => _moveDir.x = -1f;

        private void MoveRightStart() => _moveDir.x = 1f;

        private void MoveXStop() => _moveDir.x = 0f;

        private void MoveDownStart() => _moveDir.y = -1f;

        private void MoveYStop() => _moveDir.y = 0f;

        private void Jump() => OnJumpPerformed?.Invoke();

        private void JumpOff() => OnJumpOffPerformed?.Invoke();

        private void RangeAttack() => OnRangeAttackPerformed?.Invoke();

        private void MeleeAttack() => OnMeleeAttackPerformed?.Invoke();

        private void AddTriggerEvent(EventTrigger trigger, EventTriggerType type, Action callback)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener((_) => callback());
            trigger.triggers.Add(entry);
        }
    }
}
