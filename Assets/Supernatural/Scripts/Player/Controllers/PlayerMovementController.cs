using Assets.Supernatural.Scripts.AnimStateMachine;
using Assets.Supernatural.Scripts.Interfaces;
using Assets.Supernatural.Scripts.Player.AnimationStates;
using Assets.Supernatural.Scripts.Player.Controllers.Controller2D;
using UnityEditorInternal;
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
                bool isMoveDown = !controller.collisions.below && velocity.y < 0;
                return allowSlideWall && hit && nearWall && isMoveDown;
            }
        }

        //[Header("Animations")]
        //[SerializeField] private PlayerAnimController AnimController;

        [Header("Moving")]
        [SerializeField] private float moveSpeed = 3;

        [Header("OnJumpPerformed")]
        [SerializeField] private float JumpDelay = 0.1f;
        [SerializeField] private float MaxJumpHeight = 3;
        [SerializeField] private float MinJumpHeight = 1;
        [SerializeField] private float TimeToJumpApex = .4f;
        [SerializeField] private int NumberOfJumpMax = 1;
        [SerializeField] private GameObject JumpEffect;

        [Header("Wall SlideFaceToWall")]
        [SerializeField] private Transform SlidePoint;
        [SerializeField] private Vector2 wallJumpClimb;
        [SerializeField] private Vector2 wallJumpOff;
        [SerializeField] private Vector2 wallLeap;
        [SerializeField] private float wallSlideSpeedMax = 3;
        [SerializeField] private float wallStickTime = .25f;

        public float MinVelosityForLand = 3;
        public bool allowSlideWall;


        public int WallDirX => controller.collisions.left ? -1 : controller.collisions.right ? 1 : 0;
        public bool WasGrounded => _wasGrounded;
        public bool IsGrounded => controller.collisions.below;
        public bool IsHardLand => _isHardLand;

        private float velocityXSmoothing;
        private float _gravity;
        private bool _wasGrounded;
        private bool _isHardLand;

        private Vector2 velocity;
        private Vector2 _moveDir;

        private AnimationStateMachine _stateMachine;
        private PlayerController2D controller;
        private IPlayerInputController _playerInputs;
        private float accelerationTimeAirborne = .2f;
        private float accelerationTimeGrounded = .1f;
        private bool _isInit;

        private const int MAIN_ANIM_TRACK_INDEX = 1;

        public void OnEnable()
        {
            if (!_isInit)
                return;

            _playerInputs.EnableMovement();
            //_playerInputs.Player.Move.Enable();
        }

        public void Update()
        {
            HandleInput();
            ConfigureMove();

            CalculateVelocity();
            CheckLand();

            //_stateMachine.CurrentAction?.Invoke();
            Move();
            //_stateMachine.StateControl();
        }

        public void OnDisable()
        {
            _playerInputs.DisableMovement();
            //_playerInputs.Player.Move.Disable();
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
            _gravity = -(2 * MaxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
            _isInit = true;

            InitializeStateMachine(_skeletonAnimation);
        }

        public void SetDamageImpulse(Transform instigator)
        {
            var facingDirectionX = Mathf.Sign(transform.position.x - instigator.position.x);
            var facingDirectionY = Mathf.Sign(velocity.y);

            SetForce(new Vector2(
                Mathf.Clamp(Mathf.Abs(velocity.x), 10, 15) * facingDirectionX,
                Mathf.Clamp(Mathf.Abs(velocity.y), 5, 15) * -facingDirectionY)
            );
        }

        public void SetForce(Vector2 force)
        {
            velocity = (Vector3)force;
        }

        public void AddForce(Vector2 force)
        {
            velocity += force;
        }

        public void Jump()
        {
            controller.IsJumpKeyPressed = true;
            //if (_moveDir.y >= 0)
            //    _stateMachine.StateSwitch<JumpState>();
        }

        public void JumpOff()
        {
            controller.IsJumpKeyPressed = false;
            //AllServices.Instance.GetService<JumpState>().JumpOff();
        }

        public void Flip() =>
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

        private void CalculateVelocity()
        {
            velocity.x = ApplySmoothing();
            velocity.y += _gravity * Time.deltaTime;
        }

        private void HandleInput() => _moveDir = _playerInputs.ReadMovementInput();

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

        private void InitializeStateMachine(Spine.Unity.SkeletonAnimation _skeletonAnimation)
        {
            _stateMachine = new SpineStateMachine(_skeletonAnimation, MAIN_ANIM_TRACK_INDEX);
            _stateMachine.AddState<IdleState>(new IdleState());
        }
    }
}
