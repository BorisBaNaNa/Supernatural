using UnityEngine;
using UnityEngine.Windows;

public class CharacterController2D : MonoBehaviour
{
    public bool IsFlaying => _velocity.y > 0f;
    public bool IsFalling => _velocity.y < 0f;
    public bool IsGraund { get => isGraund; private set => isGraund = value; }

    [Header("Ground settings")]
    public float maxClimbAngle = 80;
    public float maxDescendAngle = 80;
    public LayerMask GraundMask;

    [Header("Move settings")]
    public LayerMask CollisionIgnore;
    public float MoveSpeed = 3;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    public float MaxJumpHeight = 5;
    public float MinJumpHeight = 0.5f;

    private Vector2 _velocity;
    private float _velocityXSmoothing;
    private CapsuleCollider2D _capsuleCollider;
    private bool isGraund;

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void FixedUpdate()
    {
        UpdateIsGraund();
    }

    private void Initialize()
    {
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void CharacterMove(Vector2 dir)
    {
        ApplySmooth(dir);
        ApplyGravity();
        transform.Translate(_velocity * Time.deltaTime);

        ApplyCollision();
    }

    public void StartJump()
    {
        if (!IsGraund) return;

        _velocity.y += Mathf.Sqrt(2 * MaxJumpHeight * Mathf.Abs(Physics2D.gravity.y));
    }

    public void StopJump()
    {
        if (!IsFlaying || IsGraund) return;

        float minJumpVelocity = Mathf.Sqrt(2 * MinJumpHeight * Mathf.Abs(Physics2D.gravity.y));
        if (_velocity.y > minJumpVelocity)
            _velocity.y = minJumpVelocity;
    }

    protected void ApplyCollision()
    {
        Vector2 CapsuleCenter = new Vector2(transform.position.x, transform.position.y + (_capsuleCollider.size.y * 0.5f));
        Collider2D[] hits = Physics2D.OverlapCapsuleAll(CapsuleCenter, _capsuleCollider.size, CapsuleDirection2D.Vertical, 0, ~CollisionIgnore);

        foreach (Collider2D hit in hits)
        {
            if (hit == _capsuleCollider) continue;
            ColliderDistance2D colliderDistance = hit.Distance(_capsuleCollider);

            if (colliderDistance.normal.y < 0 && _velocity.y > 0)
                _velocity.y = 0;

            if (colliderDistance.isOverlapped)
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
        }
    }

    protected void UpdateIsGraund()
    {
        float radius = _capsuleCollider.size.x * 0.5f;
        Vector2 circleCenter = new(transform.position.x, transform.position.y + radius - 0.01f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(circleCenter, radius, GraundMask);
        if (hits.Length == 0)
        {
            IsGraund = false;
            return;
        }

        foreach (Collider2D hit in hits)
        {
            float descendAngle = Vector2.Angle(hit.Distance(_capsuleCollider).normal, Vector2.up);
            IsGraund = descendAngle <= maxDescendAngle && _velocity.y <= 0;
            if (IsGraund) return;
        }
    }

    protected void ApplySmooth(Vector2 dir)
    {
        float targetVelocityX = dir.x * MoveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, IsGraund ? accelerationTimeGrounded : accelerationTimeAirborne);
    }

    protected void ApplyGravity()
    {
        if (!IsFlaying && IsGraund)
            _velocity.y = 0f;
        else
            _velocity += Physics2D.gravity * Time.deltaTime;
    }
}
