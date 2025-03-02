using Assets.Supernatural.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    public class PlayerAttackController : MonoBehaviour
    {
        [SerializeField] private RangeAttack _rangeAttack;
        [SerializeField] private MeleeAttack _meleeAttack;

        //private InputActions _playerInputs;
        private IPlayerInputController _playerInputs;
        private bool _isInit;

        private void Start()
        {
            _playerInputs.OnRangeAttackPerformed += RangeAttack;
            _playerInputs.OnMeleeAttackPerformed += MeleeAttack;
            //_playerInputs.Player.OnRangeAttackPerformed.performed += OnRangeAttackPerformed;
            //_playerInputs.Player.OnMeleeAttackPerformed.performed += OnMeleeAttackPerformed;
        }

        private void OnEnable()
        {
            if (!_isInit)
                return;

            _playerInputs.EnableAttack();
            //_playerInputs.Player.OnRangeAttackPerformed.Enable();
            //_playerInputs.Player.OnMeleeAttackPerformed.Enable();
        }

        private void OnDisable()
        {
            _playerInputs.DisableAttack();
            //_playerInputs.Player.OnRangeAttackPerformed.Disable();
            //_playerInputs.Player.OnMeleeAttackPerformed.Disable();
        }

        private void OnDestroy()
        {
            _playerInputs.OnRangeAttackPerformed -= RangeAttack;
            _playerInputs.OnMeleeAttackPerformed -= MeleeAttack;
            //_playerInputs.Player.OnRangeAttackPerformed.performed -= OnRangeAttackPerformed;
            //_playerInputs.Player.OnMeleeAttackPerformed.performed -= OnMeleeAttackPerformed;
        }

        public void Initialize(IPlayerInputController inputs)
        {
            _playerInputs = inputs;
            _playerInputs.EnableAttack();
            _isInit = true;
        }

        public void MeleeAttack()
        {
            //if (_meleeAttack.Attack())
            //{
            //    SoundManager.PlaySfx(_player.meleeAttackSound);
            //}
        }

        public void RangeAttack()
        {
            //if (_rangeAttack.Fire())
            //{
            //    SoundManager.PlaySfx(_player.rangeAttackSound);
            //}
        }
    }
}
