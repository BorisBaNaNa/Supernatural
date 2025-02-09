using UnityEngine;

public class PlayerController : CharacterController2D
{
    [Header("Health settings")]
    public float MaxHealth;
    public float Health { get; private set; }

    private float _currentHealth;
    protected InputActions _input;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
        InitializeInput();
    }

    protected void Update()
    {
        Vector2 dir = _input.Player.Move.ReadValue<Vector2>();
        CharacterMove(dir);
    }

    private void OnEnable()
    {
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    private void Initialize() 
    {
        _input = new InputActions();
    }

    protected void InitializeInput()
    {
        _input.Player.Jump.performed += _ => StartJump();
        _input.Player.Jump.canceled += _ => StopJump();
    }
}