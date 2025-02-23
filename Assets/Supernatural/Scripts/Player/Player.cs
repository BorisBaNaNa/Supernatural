using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

namespace Assets.Supernatural.Scripts.Player
{
    [RequireComponent(typeof(Controller2D), typeof(AudioSource))]
    public partial class Player : MonoBehaviour, ICanTakeDamage
    {
        #region Inspector
        public string CurrentState;
        public bool GodMode;

        [SerializeField] private PlayerMovementController _movementController;

        [Header("Health")]
        public int maxHealth;
        public int Health { get; set; }
        public GameObject HurtEffect;

        [Header("Sound")]
        public AudioClip jumpSound;
        public AudioClip landSound;
        public AudioClip WalkSound;
        public AudioClip wallSlideSound;
        public AudioClip hurtSound;
        public AudioClip deadSound;
        public AudioClip rangeAttackSound;
        public AudioClip meleeAttackSound;

        [Header("Option")]
        public bool allowMeleeAttack;
        public bool allowRangeAttack;
        [Range(0.01f, 1)]
        public float MinActiveStateTime = 0.1f;
        #endregion

        public PlayerWeapon CurrentWeapon { get; set; } = PlayerWeapon.None;
        public AudioSource SoundFx { get; private set; }

        private PlayerStateMachine _stateMachine;
        private InputActions _inputs;

        public void Awake()
        {
            Initialize();
        }

        public void Start()
        {
            FillingFields();

            _inputs.Player.RangeAttack.performed += _ => RangeAttack();
            _inputs.Player.MeleeAttack.performed += _ => MeleeAttack();
        }

        public void Update()
        {
            _movementController.MoveLogic();
        }

        #region Init
        private void Initialize()
        {
            _inputs = new InputActions();
            SoundFx = GetComponent<AudioSource>();
        }

        private void FillingFields()
        {
            Health = maxHealth;
            SoundFx.clip = wallSlideSound;
        }
        #endregion

        #region API

        public void MeleeAttack()
        {
            if (allowMeleeAttack)
                _stateMachine.StateSwitch<MelleAttackState>();
        }

        public void RangeAttack()
        {
            if (allowRangeAttack)
                _stateMachine.StateSwitch<RangeAttackState>();
        }

        public void RespawnAt(Vector2 pos)
        {
            transform.position = pos;

            _stateMachine.StateSwitch<RespawnState>();
        }

        public void TakeDamage(float damage, Vector2 forceDir, GameObject instigator)
        {
            if (CurrentState == "DeathState")
                return;

            _stateMachine.StateSwitch<TakeDamageState>();

            if (GodMode)
                return;

            Health -= (int)damage;

            if (Health <= 0)
                Kill();

            if (forceDir.x == 0 && forceDir.y == 0)
                return;

            _movementController.SetDamageImpulse(instigator.transform);
        }

        public void GiveHealth(int hearthToGive, GameObject instigator)
        {
            Health = Mathf.Min(Health + hearthToGive, maxHealth);
            //GameManager.Instance.ShowFloatingText("+" + hearthToGive, transform.position, Color.red);
        }

        public void Kill() =>
            _stateMachine.StateSwitch<DeathState>();

        public void GameFinish() =>
            _stateMachine.StateSwitch<FinishState>();
        #endregion
    }
}
