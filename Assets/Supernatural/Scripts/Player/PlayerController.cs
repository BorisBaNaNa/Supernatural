using Assets.Supernatural.Scripts.AnimStateMachine;
using Assets.Supernatural.Scripts.Infrastructure;
using Assets.Supernatural.Scripts.Interfaces;
using Assets.Supernatural.Scripts.Player.Abilities;
using Assets.Supernatural.Scripts.Player.AnimationStates.Once;
using Assets.Supernatural.Scripts.Player.Configs;
using Assets.Supernatural.Scripts.Player.Controllers;
using SaintsField;
using SaintsField.Playa;
using Spine.Unity;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player
{
    [RequireComponent(typeof(PleyerHealthController), typeof(PlayerMovementController), typeof(PlayerAttackController))]
    public partial class PlayerController : MonoBehaviour, ICanTakeDamage, ICanTakeHealth
    {
        public AnimationStateMachine AnimStateMachine => _animStateMachine;

        [LayoutStart("Components", ELayout.TitleOut)]
        [SerializeField] private PleyerHealthController _healthController;
        [SerializeField] private PlayerMovementController _movementController;
        [SerializeField] private PlayerAttackController _attackController;

        [LayoutStart("Anim Settings", ELayout.TitleOut)]
        [SerializeField] private SkeletonAnimation _skeletonAnimation;
        [SerializeField] private PlayerMainAnimationsReferences _animationsReferences;

        [LayoutStart("Abilities", ELayout.TitleOut)]
        [SerializeReference, HideIf(nameof(AbilityIsNull)), Ordered(1)] private IUltimateAbility _ultimateAbility;

        private bool AbilityIsNull => _ultimateAbility == null;

        private AnimationStateMachine _animStateMachine;
        private IPlayerInputController _inputController;
        private bool _isDead;

        private const int MAIN_ANIM_TRACK_INDEX = 0;

        public void Awake()
        {
            _healthController.OnDie += Kill;

            Initialize();
        }

        private void Update()
        {
            _animStateMachine.Update();

            if (_ultimateAbility.IsActive)
                _ultimateAbility.UpdateAbility(this);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
                TakeDamage(10, Vector2.zero, gameObject);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
                StartUltimate();
        }

        public void OnEnable()
        {
            _inputController.Enable();
        }

        public void OnDisable()
        {
            _inputController.Disable();
        }

        public void OnDestroy()
        {
            _healthController.OnDie -= Kill;
        }

        public void TakeDamage(float damage, Vector2 force, GameObject instigator)//Нах тут forceDir??????
        {
            if (_isDead)
                return;

            if (_ultimateAbility.IsActive && _ultimateAbility.HandleDamageTaken(this, damage, force, instigator))
                return;

            BaseHandleDamageTaken(damage, instigator);
        }

        public void BaseHandleDamageTaken(float damage, GameObject instigator)
        {
            _animStateMachine.StateSwitch<TakeDamageState>();
            _healthController.TakeDamage(damage);
            _movementController.SetDamageImpulse(instigator.transform);
        }

        public void TakeHealth(float takenHearth)
        {
            if (_isDead)
                return;

            _healthController.TakeHealth(takenHearth);
        }

        public void RespawnAt(Vector2 pos)
        {
            _isDead = false;
            EnableMovement();
            _inputController.Enable();
            _healthController.Initialize();
            transform.position = pos;
        }

        public void Kill()
        {
            _isDead = true;

            if (_ultimateAbility.IsActive)
                _ultimateAbility.Deactivate(this);

            DisableMovement();
            _inputController.Disable();
            _animStateMachine.StateSwitch<DeathState>();
        }

        public void EnableMovement() => _movementController.Enable();

        public void DisableMovement() => _movementController.Disable();

        public void EnableAttack() => _attackController.Enable();

        public void DisableAttack() => _attackController.Disable();

        public void StartUltimate()
        {
            if (_isDead)
                return;

            _ultimateAbility.Activate(this);
        }

        #region Init
        private void Initialize()
        {
            InitializeStateMachine();
            _healthController.Initialize();

            _inputController = ServiceLocator.GetService<IPlayerInputController>();
            _inputController.Initialize();
            _movementController.Initialize(_inputController, _skeletonAnimation);
            _attackController.Initialize(_inputController, _skeletonAnimation);
        }

        private void InitializeStateMachine()
        {
            _animStateMachine = new SpineStateMachine(_skeletonAnimation, MAIN_ANIM_TRACK_INDEX);
            _animStateMachine.AddState(new TakeDamageState(_animationsReferences));
            _animStateMachine.AddState(new DeathState(_animationsReferences));
            _animStateMachine.AddState(new UltimateState(_animationsReferences));
        }
        #endregion
    }

#if UNITY_EDITOR
    public partial class PlayerController
    {
        [Layout("Abilities", ELayout.TitleOut)]
        [SerializeField, Dropdown(nameof(GetUltimateTypes)), Ordered(0)] private string _ultimateAbilityType;

        [SerializeField, HideInInspector] private string _lastAbilityType = string.Empty;

        private void OnValidate()
        {
            if (_healthController == null)
                _healthController = GetComponent<PleyerHealthController>();
            if (_movementController == null)
                _movementController = GetComponent<PlayerMovementController>();
            if (_attackController == null)
                _attackController = GetComponent<PlayerAttackController>();

            ValidateAbilityInstance();
        }

        private void ValidateAbilityInstance()
        {
            if (_lastAbilityType == _ultimateAbilityType)
                return;

            _lastAbilityType = _ultimateAbilityType;

            if (string.IsNullOrEmpty(_ultimateAbilityType))
                _ultimateAbility = null;
            else
            {
                var type = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == _ultimateAbilityType);
                _ultimateAbility = (IUltimateAbility)Activator.CreateInstance(type);
            }
        }

        private static DropdownList<string> GetUltimateTypes()
        {
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(UltimateAbilityBase)));

            DropdownList<string> result = new() { { "None", "" } };
            foreach (var type in types)
                result.Add(type.Name, type.Name);

            return result;
        }
    }
#endif
}
