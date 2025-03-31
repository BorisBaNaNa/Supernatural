using Assets.Supernatural.Scripts.AnimStateMachine;
using Assets.Supernatural.Scripts.Interfaces;
using Assets.Supernatural.Scripts.Player.AnimationStates.Movement;
using Assets.Supernatural.Scripts.Player.Configs;
using Assets.Supernatural.Scripts.Player.Controllers.Controller2D;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        public bool IsClimbingOnWall => _isEnabled 
            && (controller.collisions.hasWallAbove || controller.collisions.hasWallBelow) 
            && WallDirX == Mathf.Sign(transform.localScale.x);

        [Header("Moving")]
        [SerializeField] private float _flySpeed = 6f;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _climbSpeed = 2.5f;

        [Header("OnJumpPerformed")]
        [SerializeField] private float _jumpDelay = 0.1f;
        [SerializeField] private float _maxJumpHeight = 3;
        [SerializeField] private float _minJumpHeight = 1;
        [SerializeField] private float _timeToJumpApex = .4f;
        [SerializeField] private int _maxJumpCount = 2;
        [SerializeField] private GameObject _jumpEffect;

        [Header("Wall SlideFaceToWall")]
        [SerializeField] private Transform SlidePoint;
        [SerializeField] private Vector2 wallJumpClimb;
        [SerializeField] private float wallSlideSpeedMax = 3;
        [SerializeField] private float wallStickTime = .25f;

        [Header("Anim Settings")]
        [SerializeField] private PlayerMovementAnimationsReferences _animationsReferences;

        public float MinVelosityForLand = 3;
        public bool AllowClimbWall;


        public int WallDirX => controller.collisions.left ? -1 : controller.collisions.right ? 1 : 0;
        public bool WasGrounded => _wasGrounded;
        public bool IsGrounded => controller.collisions.below;

        private float velocityXSmoothing;
        private float velocityYSmoothing;
        private float _gravity;
        private float _maxJumpVelocity;
        private float _minJumpVelocity;
        private int _jumpCount;
        private bool _wasGrounded;
        private bool _isInit;

        private Vector2 _currentJumpVelocity = Vector2.zero;
        private Vector2 _velocity;
        private Vector2 _inputDir;

        private AnimationStateMachine _stateMachine;
        private PlayerController2D controller;
        private CachedMovementData _cachedMovementData = new();
        private IPlayerInputController _playerInputs;
        private float accelerationTimeAirborne = .2f;
        private float accelerationTimeGrounded = .1f;
        private bool _isEnabled;

        private const int MAIN_ANIM_TRACK_INDEX = 1;

        public void OnEnable()
        {
            if (!_isInit)
                return;

            Enable();
        }

        public void OnDisable()
        {
            Disable();
        }

        public void Update()
        {
            HandleInput();
            ConfigureInput();
            CalculateVelocity();
            Move();
            CheckCollisions();
            SaveCachedData();

            _stateMachine.Update();
            SaveUpdateData();
        }

        private void OnDestroy()
        {
            _playerInputs.OnJumpPerformed -= Jump;
            _playerInputs.OnJumpOffPerformed -= JumpOff;
        }

        public void Initialize(IPlayerInputController inputs, Spine.Unity.SkeletonAnimation _skeletonAnimation)
        {
            _playerInputs = inputs;
            _playerInputs.EnableMovement();
            _playerInputs.OnJumpPerformed += Jump;
            _playerInputs.OnJumpOffPerformed += JumpOff;

            controller = GetComponent<PlayerController2D>();
            _gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_timeToJumpApex, 2);
            _maxJumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;
            _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * _minJumpHeight);

            InitializeStateMachine(_skeletonAnimation);
            Enable();
            _isInit = true;
        }

        public void Enable()
        {
            _isEnabled = true;
            _playerInputs.EnableMovement();
            _stateMachine.StateSwitch<IdleState>();
        }

        public void Disable()
        {
            _isEnabled = false;
            _playerInputs.DisableMovement();
            _stateMachine.DropCurrentState();
        }

        public void SetDamageImpulse(Transform instigator)
        {
            var facingDirectionX = Mathf.Sign(transform.position.x - instigator.position.x);
            var facingDirectionY = Mathf.Sign(_velocity.y);

            SetForce(new Vector2(
                Mathf.Clamp(Mathf.Abs(_velocity.x), 10, 15) * facingDirectionX,
                Mathf.Clamp(Mathf.Abs(_velocity.y), 5, 15) * -facingDirectionY)
            );
        }

        public void SetForce(Vector2 force)
        {
            _velocity = (Vector3)force;
        }

        public void AddForce(Vector2 force)
        {
            _velocity += force;
        }

        private void InitializeStateMachine(Spine.Unity.SkeletonAnimation _skeletonAnimation)
        {
            _stateMachine = new SpineStateMachine(_skeletonAnimation, MAIN_ANIM_TRACK_INDEX);
            _stateMachine.AddState(new IdleState(_animationsReferences, _cachedMovementData));
            _stateMachine.AddState(new WalkState(_animationsReferences, _cachedMovementData));
            _stateMachine.AddState(new JumpState(_animationsReferences, _cachedMovementData));
            _stateMachine.AddState(new ClimbState(_animationsReferences, _cachedMovementData));
        }

        private void HandleInput()
        {
            _inputDir = _playerInputs.ReadMovementInput();
        }

        private void ConfigureInput()
        {
            if (_inputDir.x != 0 && Mathf.Sign(transform.localScale.x) != _inputDir.x)
                Flip();
        }

        private void CalculateVelocity()
        {
            if (_currentJumpVelocity != Vector2.zero)
            {
                _velocity.x = _currentJumpVelocity.x;
                _velocity.y = _currentJumpVelocity.y;
                _currentJumpVelocity = Vector2.zero;
                return;
            }

            _velocity.x = ApplySmoothingX(_inputDir.x * (IsGrounded ? _moveSpeed : _flySpeed));

            if (IsClimbingOnWall)
            {
                if (!controller.collisions.hasWallAbove && _inputDir.y > 0
                    || !controller.collisions.hasWallBelow && _inputDir.y < 0)
                    _velocity.y = ApplySmoothingY(0);
                else
                    _velocity.y = ApplySmoothingY(_inputDir.y * _climbSpeed);
            }
            else
                _velocity.y += ApplyGravity();
        }

        private void Move() => controller.Move(_velocity * Time.deltaTime, _inputDir);

        private void CheckCollisions()
        {
            if (controller.collisions.above || IsGrounded)
            {
                _velocity.y = 0;

                if (IsGrounded)
                    _jumpCount = 0;
            }
        }

        private void Flip() =>
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

        private float ApplySmoothingX(float targetVelocityX)
        {
            float smoothTime = IsGrounded ? accelerationTimeGrounded : accelerationTimeAirborne;
            return Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
        }

        private float ApplySmoothingY(float targetVelocityY)
        {
            float smoothTime = accelerationTimeGrounded;
            return Mathf.SmoothDamp(_velocity.y, targetVelocityY, ref velocityYSmoothing, smoothTime);
        }

        private float ApplyGravity()
        {
            return _gravity * Time.deltaTime;
        }

        private void SaveUpdateData()
        {
            _wasGrounded = IsGrounded;
        }

        private void SaveCachedData()
        {
            _cachedMovementData.Velocity = _velocity;
            _cachedMovementData.InputDir = _inputDir;
            _cachedMovementData.IsGrounded = IsGrounded;
            _cachedMovementData.IsClimbingOnWall = IsClimbingOnWall;
        }

        public void Jump()
        {
            controller.IsJumpKeyPressed = true;

            if (IsClimbingOnWall)
                JumpClimbing();
            else if (IsGrounded || _jumpCount < _maxJumpCount)
            {
                float jumpVelocity = _jumpCount == 0 && IsGrounded ? _maxJumpVelocity : _minJumpVelocity;
                Jump(jumpVelocity);
            }
        }

        public void JumpOff()
        {
            controller.IsJumpKeyPressed = false;

            if (_velocity.y > _minJumpVelocity)
                _velocity.y = _minJumpVelocity;
        }

        private void Jump(float jumpVelocity)
        {
            if (IsGrounded)
                _jumpCount++;
            else
                _jumpCount = _maxJumpCount;

            _stateMachine.StateSwitch<JumpState>();
            _currentJumpVelocity.y = jumpVelocity;

            if (_jumpEffect != null)
                Object.Instantiate(_jumpEffect, transform.position, transform.rotation);
            //SoundManager.PlaySfx(_player.jumpSound);
        }

        private void JumpClimbing()
        {
            float climbJumpVelocity = _maxJumpVelocity;

            if (_inputDir.y < 0)
            {
                _currentJumpVelocity.y = -_minJumpVelocity;
                return;
            }

            if (_inputDir.x != WallDirX)
                _currentJumpVelocity.x = -WallDirX * climbJumpVelocity;

            Flip();
            Jump(climbJumpVelocity);
            //SoundManager.PlaySfx(jumpSound);
        }

        public class CachedMovementData
        {
            public Vector3 Velocity { get; set; }
            public Vector2 InputDir { get; set; }
            public bool IsGrounded { get; set; }
            public bool IsClimbingOnWall { get; set; }
        }
    }
}
