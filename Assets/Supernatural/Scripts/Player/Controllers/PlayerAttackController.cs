using Assets.Supernatural.Scripts.AnimStateMachine;
using Assets.Supernatural.Scripts.Interfaces;
using Assets.Supernatural.Scripts.Player.AnimationStates.Once;
using Assets.Supernatural.Scripts.Player.Configs;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    public class PlayerAttackController : MonoBehaviour
    {
        [SerializeField] private RangeAttack _rangeAttack;
        [SerializeField] private MeleeAttack _meleeAttack;

        [Header("Anim Settings")]
        [SerializeField] private PlayerAttackAnimationsReferences _animationsReferences;

        //private InputActions _playerInputs;
        private AnimationStateMachine _animStateMachine;
        private IPlayerInputController _playerInputs;
        private bool _isInit;

        private const int MAIN_ANIM_TRACK_INDEX = 2;

        private void Awake()
        {

        }

        private void Start()
        {
            _playerInputs.OnRangeAttackPerformed += RangeAttack;
            _playerInputs.OnMeleeAttackPerformed += MeleeAttack;
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
        }

        public void Initialize(IPlayerInputController inputs, Spine.Unity.SkeletonAnimation _skeletonAnimation)
        {
            _playerInputs = inputs;
            _playerInputs.EnableAttack();
            InitializeStateMachine(_skeletonAnimation);
            _isInit = true;
        }

        public void MeleeAttack()
        {
            if (_meleeAttack.AttackIsReady)
            {
                _animStateMachine.StateSwitch<MeleeAttackState>();
            }
        }

        public void RangeAttack()
        {
            if (_rangeAttack.AttackIsReady)
            {
                _animStateMachine.StateSwitch<RangeAttackState>();
            }
        }

        public void Enable()
        {
            _playerInputs.EnableAttack();
        }

        public void Disable()
        {
            _playerInputs.DisableAttack();
        }


        private void InitializeStateMachine(Spine.Unity.SkeletonAnimation skeletonAnimation)
        {
            _animStateMachine = new SpineStateMachine(skeletonAnimation, MAIN_ANIM_TRACK_INDEX);
            _animStateMachine.AddState(new RangeAttackState(_animationsReferences, _rangeAttack.Fire));
            _animStateMachine.AddState(new MeleeAttackState(_animationsReferences, _meleeAttack.Attack));
        }
    }
}
