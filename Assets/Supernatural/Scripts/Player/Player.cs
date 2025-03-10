using Assets.Supernatural.Scripts.AnimStateMachine;
using Assets.Supernatural.Scripts.Infrastructure;
using Assets.Supernatural.Scripts.Interfaces;
using Assets.Supernatural.Scripts.Player.AnimationStates;
using Assets.Supernatural.Scripts.Player.Controllers;
using SoundSystem.Scripts.Infrastructure.Manages;
using Spine.Unity;
using System;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player
{
    [RequireComponent(typeof(PleyerHealthController), typeof(PlayerMovementController), typeof(PlayerAttackController))]
    public partial class Player : MonoBehaviour, ICanTakeDamage, ICanTakeHealth
    {
        [SerializeField] private PleyerHealthController _healthController;
        [SerializeField] private PlayerMovementController _movementController;
        [SerializeField] private PlayerAttackController _attackController;
        [SerializeField] private SkeletonAnimation _skeletonAnimation;

        private AudioSource _soundFxSource;
        private SoundGroupsController _soundController;
        private IPlayerInputController _inputController;
        private AnimationStateMachine _stateMachine;

        private const int MAIN_ANIM_TRACK_INDEX = 0;

        public void Awake()
        {
            _healthController.OnDie += Kill;

            Initialize();
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

        #region Init
        private void Initialize()
        {
            InitializeStateMachine();
            InitializeSoundSystem();
            _healthController.Initialize();

            _inputController = ServiceLocator.GetService<IPlayerInputController>();
            _inputController.Initialize();
            _movementController.Initialize(_inputController, _skeletonAnimation);
            _attackController.Initialize(_inputController, _skeletonAnimation);
        }

        private void InitializeStateMachine()
        {
            _stateMachine = new SpineStateMachine(_skeletonAnimation, MAIN_ANIM_TRACK_INDEX);
        }

        private void InitializeSoundSystem()
        {
            //_soundController = ServiceLocator.GetService<SoundGroupsService>().SoundGroupsController;
            //_soundFxSource = _soundController.CreateAudioSource(SoundSystem.Scripts.Infrastructure.SoundGroups.Sound, gameObject);
        }
        #endregion

        public void TakeDamage(float damage, Vector2 force, GameObject instigator)//Нах тут forceDir??????
        {
            //_stateMachine.StateSwitch<TakeDamageState>();
            _healthController.TakeDamage(damage);
            _movementController.SetDamageImpulse(instigator.transform);
        }

        public void TakeHealth(int takenHearth)
        {
            _healthController.TakeHealth(takenHearth);
        }

        public void RespawnAt(Vector2 pos)
        {
            transform.position = pos;

            //_stateMachine.StateSwitch<RespawnState>();
        }

        public void Kill()
        {
            //_stateMachine.StateSwitch<DeathState>();
        }

        public void GameFinish()
        {
            //_stateMachine.StateSwitch<FinishState>();
        }
    }
}
