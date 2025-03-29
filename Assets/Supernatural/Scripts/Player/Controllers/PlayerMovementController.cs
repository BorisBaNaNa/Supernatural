using Assets.Supernatural.Scripts.AnimStateMachine;
using Assets.Supernatural.Scripts.Interfaces;
using Assets.Supernatural.Scripts.Player.AnimationStates.Movement;
using Assets.Supernatural.Scripts.Player.Controllers.Controller2D;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        public bool IsSliding
        {
            get
            {
                Vector2 dir = new(controller.collisions.left ? -1 : 1, 0);
                RaycastHit2D hit = Physics2D.Raycast(SlidePoint.position, dir, 0.6f, LayerMask.GetMask("Ground"));

                bool nearWall = controller.collisions.left || controller.collisions.right;
                bool isMoveDown = !controller.collisions.below && _velocity.y < 0;
                return allowSlideWall && hit && nearWall && isMoveDown;
            }
        }

        [Header("Moving")]
        [SerializeField] private float moveSpeed = 3;

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
        [SerializeField] private Vector2 wallJumpOff;
        [SerializeField] private Vector2 wallLeap;
        [SerializeField] private float wallSlideSpeedMax = 3;
        [SerializeField] private float wallStickTime = .25f;

        [Header("Anim Settings")]
        [SerializeField] private PlayerMovementAnimationsReferences _animationsReferences;

        public float MinVelosityForLand = 3;
        public bool allowSlideWall;


        public int WallDirX => controller.collisions.left ? -1 : controller.collisions.right ? 1 : 0;
        public bool WasGrounded => _wasGrounded;
        public bool IsGrounded => controller.collisions.below;
        public bool IsHardLand => _isHardLand;

        private float velocityXSmoothing;
        private float _gravity;
        private float _maxJumpVelocity;
        private float _minJumpVelocity;
        private int _jumpCount;
        private bool _wasGrounded;
        private bool _isHardLand;
        private bool _isInit;
        private bool _isCrouching;
        private bool _wasCrouched;

        private Vector2 _velocity;
        private Vector2 _inputDir;

        private AnimationStateMachine _stateMachine;
        private PlayerController2D controller;
        private IPlayerInputController _playerInputs;
        private float accelerationTimeAirborne = .2f;
        private float accelerationTimeGrounded = .1f;

        private const int MAIN_ANIM_TRACK_INDEX = 1;

        public void OnEnable()
        {
            if (!_isInit)
                return;

            _playerInputs.EnableMovement();
        }

        public void Update()
        {
            HandleInput();
            ConfigureInput();

            CalculateVelocity();
            CheckLand();

            _stateMachine.Update();
            Move();

            StatesControl();
            SaveUpdateData();
        }

        public void OnDisable()
        {
            _playerInputs.DisableMovement();
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
            _isInit = true;
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
            _stateMachine.AddState<IdleState>(new IdleState(_animationsReferences));
            _stateMachine.AddState<WalkState>(new WalkState(_animationsReferences));
            _stateMachine.AddState<CrouchState>(new CrouchState(_animationsReferences));
            _stateMachine.AddState<JumpState>(new JumpState(_animationsReferences));
        }

        private void HandleInput()
        {
            _inputDir = _playerInputs.ReadMovementInput();
            _isCrouching = _inputDir.y < 0 && _velocity.y == 0;
        }

        private void ConfigureInput()
        {
            if (_inputDir.x != 0 && Mathf.Sign(transform.localScale.x) != _inputDir.x)
                Flip();

            if (_inputDir.magnitude > 1f)
                _inputDir.Normalize();
        }

        private void Flip() =>
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

        private void CalculateVelocity()
        {
            _velocity.x = ApplySmoothing();
            _velocity.y += _gravity * Time.deltaTime;
        }

        private float ApplySmoothing()
        {
            float targetVelocityX = _isCrouching ? 0 : _inputDir.x * moveSpeed;
            float smoothTime = (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne;
            return Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
        }

        private void CheckLand()
        {
            _isHardLand = Mathf.Abs(_velocity.y) > MinVelosityForLand;
        }

        private void Move()
        {
            if (_isCrouching != _wasCrouched)
                controller.Crouch(_isCrouching && !_wasCrouched);

            controller.Move(_velocity * Time.deltaTime, _inputDir);

            if (controller.collisions.above || IsGrounded)
            {
                _velocity.y = 0;
                _jumpCount = 0;
            }
        }

        private void StatesControl()
        {
            if (_velocity.y == 0)
            {
                if (Mathf.Abs(_velocity.x) >= 1f)
                {
                    if (IsGrounded && _inputDir.x != 0)
                        _stateMachine.StateSwitch<WalkState>();
                }
                else
                {
                    if (_inputDir == Vector2.zero)
                        _stateMachine.StateSwitch<IdleState>();
                    else if (_inputDir.y < 0)
                        _stateMachine.StateSwitch<CrouchState>();
                }
            }
        }

        private void SaveUpdateData()
        {
            _wasGrounded = IsGrounded;
            _wasCrouched = _isCrouching;
        }

        public void Jump()
        {
            controller.IsJumpKeyPressed = true;

            if (IsSliding)
                JumpSliding();
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
            _velocity.y = jumpVelocity;

            if (_jumpEffect != null)
                Object.Instantiate(_jumpEffect, transform.position, transform.rotation);
            //SoundManager.PlaySfx(_player.jumpSound);
        }

        private void JumpSliding()
        {
            if (_inputDir.x == WallDirX)
            {
                _velocity.x = -WallDirX * wallJumpClimb.x;
                _velocity.y = wallJumpClimb.y;
            }
            else if (_inputDir.x == 0)
            {
                _velocity.x = -WallDirX * wallJumpOff.x;
                _velocity.y = wallJumpOff.y;
                Flip();
            }
            else
            {
                _velocity.x = -WallDirX * wallLeap.x;
                _velocity.y = wallLeap.y;
            }

            _jumpCount = _maxJumpCount;
            //SoundManager.PlaySfx(jumpSound);
        }

    }
}
