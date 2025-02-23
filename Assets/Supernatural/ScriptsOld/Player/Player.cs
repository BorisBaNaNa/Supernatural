using System;
using UnityEngine;

[RequireComponent(typeof(Controller2D), typeof(AudioSource))]
public partial class Player : MonoBehaviour, ICanTakeDamage
{
    #region Inspector
    public string CurrentState;
    public bool GodMode;

    [Header("Animations")]
    public PlayerAnimController AnimController;

    [Header("Moving")]
    public float moveSpeed = 3;

    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;

    [Header("Jump")]
    public float JumpDelay = 0.1f;
    public float MaxJumpHeight = 3;
    public float MinJumpHeight = 1;
    public float TimeToJumpApex = .4f;
    public int NumberOfJumpMax = 1;
    public GameObject JumpEffect;

    [Header("Wall SlideFaceToWall")]
    public Transform SlidePoint;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;

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
    public bool allowSlideWall;
    [Range(0.01f, 1)]
    public float MinActiveStateTime = 0.1f;
    public float MinVelosityForLand = 3;
    #endregion

    public PlayerWeapon CurrentWeapon { get; set; } = PlayerWeapon.None;
    public Vector2 Velocity { get => velocity; set => velocity = value; }
    public float Gravity { get => _gravity; set => _gravity = value; }
    public Vector2 MoveDir { get => _moveDir; set => _moveDir = value; }
    public AudioSource SoundFx { get; private set; }

    public Controller2D Controller => controller;
    public GameObject LastInstigator => _lastInstigator;
    public InputActions Inputs => _inputs;
    public int WallDirX => controller.collisions.left ? -1 : 1;
    public bool IsSliding
    {
        get
        {
            Vector2 dir = new(controller.collisions.left ? -1 : 1, 0);
            RaycastHit2D hit = Physics2D.Raycast(SlidePoint.position, dir, 0.6f, LayerMask.GetMask("Ground"));

            bool nearWall = controller.collisions.left || controller.collisions.right;
            bool isMoveDown = !controller.collisions.below && velocity.y < 0;
            return allowSlideWall && hit && nearWall && isMoveDown;
        }
    }
    public bool WasGrounded => _wasGrounded;
    public bool IsGrounded => controller.collisions.below;
    public bool IsHardLand => _isHardLand;

    private float velocityXSmoothing;
    private float _gravity;
    private bool _wasGrounded;
    private bool _isHardLand;

    private Vector2 velocity;
    private Vector2 _moveDir;

    private Controller2D controller;

    private InputActions _inputs;
    private PlayerStateMachine _stateMachine;
    private GameObject _lastInstigator;

    public void Awake()
    {
        Initialize();
    }

    public void Start()
    {
        InitInputs();
        FillingFields();
    }

    public void Update()
    {
        #if !UNITY_ANDROID
        HandleInput();
        #endif
        ConfigureMove();

        CalculateVelocity();
        CheckLand();

        _stateMachine.CurrentAction?.Invoke();
        Move();
        _stateMachine.StateControl();
    }

    public void OnEnable()
    {
        _inputs.Player.Enable();
    }

    public void OnDisable()
    {
        _inputs.Player.Disable();
    }

    #region Init
    private void Initialize()
    {
        controller = GetComponent<Controller2D>();
        _inputs = new InputActions();

        _stateMachine = new PlayerStateMachine(this);
        _stateMachine.StateSwitch<IdleState>();

        SoundFx = GetComponent<AudioSource>();
    }

    private void InitInputs()
    {
        _inputs.Player.Jump.performed += _ => Jump();
        _inputs.Player.Jump.canceled += _ => JumpOff();
        _inputs.Player.RangeAttack.performed += _ => RangeAttack();
        _inputs.Player.MeleeAttack.performed += _ => MeleeAttack();
    }

    private void FillingFields()
    {
        Health = maxHealth;
        SoundFx.clip = wallSlideSound;
    }
    #endregion

    #region API
    public void Jump()
    {
        controller.IsJumpKeyPressed = true;
        if (_moveDir.y >= 0)
            _stateMachine.StateSwitch<JumpState>();
    }

    public void JumpOff() {
        controller.IsJumpKeyPressed = false;
        AllServices.Instance.GetService<JumpState>().JumpOff();
    }

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

    public void SetForce(Vector2 force)
    {
        velocity = (Vector3)force;
        //		controller.SetForce(forceDir);
    }

    public void AddForce(Vector2 force)
    {
        velocity += force;
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

        _lastInstigator = instigator;
        _stateMachine.StateSwitch<TakeDamageState>();

        if (GodMode)
            return;

        Health -= (int)damage;

        if (Health <= 0)
            Kill();

        if (forceDir.x == 0 && forceDir.y == 0)
            return;

        //set forceDir to player
        var facingDirectionX = Mathf.Sign(transform.position.x - instigator.transform.position.x);
        var facingDirectionY = Mathf.Sign(velocity.y);

        SetForce(new Vector2(Mathf.Clamp(Mathf.Abs(velocity.x), 10, 15) * facingDirectionX,
            Mathf.Clamp(Mathf.Abs(velocity.y), 5, 15) * -facingDirectionY));
    }

    public void GiveHealth(int hearthToGive, GameObject instigator)
    {
        Health = Mathf.Min(Health + hearthToGive, maxHealth);
        //GameManager.Instance.ShowFloatingText("+" + hearthToGive, transform.position, Color.red);
    }

    public void Kill()=>
        _stateMachine.StateSwitch<DeathState>();

    public void GameFinish() => 
        _stateMachine.StateSwitch<FinishState>();

    public void Flip() => 
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    #endregion

    private void CalculateVelocity()
    {
        velocity.x = ApplySmoothing();
        velocity.y += _gravity * Time.deltaTime;
    }

    #if !UNITY_ANDROID
    private void HandleInput() =>
        _moveDir = _inputs.Player.Move.ReadValue<Vector2>();
    #endif

    private void ConfigureMove()
    {
        if (_moveDir.x != 0 && Mathf.Sign(transform.localScale.x) != _moveDir.x)
            Flip();

        if (_moveDir.magnitude > 1f)
            _moveDir.Normalize();
    }

    private float ApplySmoothing()
    {
        float targetVelocityX = _moveDir.x * moveSpeed;
        float smoothTime = (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne;
        return Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
    }

    private void CheckLand()
    {
        _isHardLand = Mathf.Abs(velocity.y) > MinVelosityForLand;
        _wasGrounded = IsGrounded;
    }

    private void Move()
    {
        controller.Move(velocity * Time.deltaTime, _moveDir);
        if (controller.collisions.above || IsGrounded)
            velocity.y = 0;
    }
}