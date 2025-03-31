using Assets.Supernatural.Scripts.Interfaces;
using Assets.Supernatural.Scripts.Player.AnimationStates.Movement;
using UnityEditorInternal;
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

        private const int MAIN_ANIM_TRACK_INDEX = 2;

        private void Start()
        {
            _playerInputs.OnRangeAttackPerformed += RangeAttack;
            _playerInputs.OnMeleeAttackPerformed += MeleeAttack;
            //_playerInputs.PlayerController.OnRangeAttackPerformed.performed += OnRangeAttackPerformed;
            //_playerInputs.PlayerController.OnMeleeAttackPerformed.performed += OnMeleeAttackPerformed;
        }

        private void OnEnable()
        {
            if (!_isInit)
                return;

            Enable();
        }

        private void OnDisable()
        {
            Disable();
        }

        private void OnDestroy()
        {
            _playerInputs.OnRangeAttackPerformed -= RangeAttack;
            _playerInputs.OnMeleeAttackPerformed -= MeleeAttack;
            //_playerInputs.PlayerController.OnRangeAttackPerformed.performed -= OnRangeAttackPerformed;
            //_playerInputs.PlayerController.OnMeleeAttackPerformed.performed -= OnMeleeAttackPerformed;
        }

        public void Initialize(IPlayerInputController inputs, Spine.Unity.SkeletonAnimation _skeletonAnimation)
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

        public void Enable()
        {
            _playerInputs.EnableAttack();
            //_playerInputs.PlayerController.OnRangeAttackPerformed.Enable();
            //_playerInputs.PlayerController.OnMeleeAttackPerformed.Enable();
        }

        public void Disable()
        {
            _playerInputs.DisableAttack();
            //_playerInputs.PlayerController.OnRangeAttackPerformed.Disable();
            //_playerInputs.PlayerController.OnMeleeAttackPerformed.Disable();
            //_stateMachine.DropCurrentState();
        }
    }
}
