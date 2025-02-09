using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class EnemyAI : MonoBehaviour, ICanTakeDamage, IPlayerRespawnListener
{
    [Header("Behavior")]
    public EnemyAnimController EnemyAnim;
    public float gravity = 35f;
    [Tooltip("allow push the enemy back when hit by player")]
    public bool pushEnemyBack = true;
    Vector2 pushForce;
    public GameObject spawnItemWhenDead;

    [Header("Moving")]
    public float moveSpeed = 3;
    public bool ignoreCheckGroundAhead = false;
    public GameObject DestroyEffect;

    public enum HealthType { HitToKill, HealthAmount }
    [Header("Health")]

    public HealthType healthType;
    public int maxHitToKill = 1;
    [HideInInspector]
    public int currentHitLeft;

    public float health;
    float currentHealth;
    public int pointToGivePlayer;
    public GameObject HurtEffect;

    [Header("Sound")]
    public AudioClip hurtSound;
    public AudioClip deadSound;
    public AudioClip FireSound;

    [Header("Projectile")]
    public bool isUseProjectile;
    public LayerMask shootableLayer;
    public Transform PointSpawn;
    public Projectile projectile;
    public float fireRate = 1f;
    public float FireDelay = 0.5f;
    public float detectDistance = 10f;
    float _fireIn;
    float _fireDelayWait;

    public bool isPlaying { get; set; }
    public bool isSocking { get; set; }
    public bool isDead { get; set; }

    [HideInInspector]
    public Controller2D controller;

    private Vector3 velocity;
    private Vector2 _direction;
    private Vector2 _startPosition; //set this enemy back to the first position when Player spawn to check point
    private Vector2 _startScale;    //set this enemy back to the first position when Player spawn to check point

    private Player _player;
    private AudioSource _audioSource;

    // Use this for initialization
    public virtual void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _player = AllServices.Instance.GetService<LevelManager>().Player;
        controller = GetComponent<Controller2D>();
        _direction = Vector2.left;
        _startPosition = transform.position;
        _startScale = transform.localScale;
        _fireIn = fireRate;
        _fireDelayWait = FireDelay;
        currentHealth = health;
        currentHitLeft = maxHitToKill;

        isPlaying = true;
        isSocking = false;

        if (EnemyAnim == null)
            Debug.LogError("EnemyAnim is null");

        GetComponent<GiveDamageToPlayer>().GiveDamageAction = () => EnemyAnim.PlayMeleeAttackAnim();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (_player.CurrentState == "FinishState")
            enabled = false;

        if (!isPlaying || isSocking)
            return;

        if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)
            || (!ignoreCheckGroundAhead && !controller.collisions.isGrounedAhead && controller.collisions.below))
        {

            _direction = -_direction;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }



        if (isUseProjectile)
        {
            _fireIn -= Time.deltaTime;
            var position = PointSpawn != null ? PointSpawn.position : transform.position;
            var hit = Physics2D.Raycast(position, _direction, detectDistance, shootableLayer);
            if (hit)
            {
                if (hit.collider.gameObject.GetComponent<Player>() != null)
                {
                    if (_fireDelayWait <= 0)
                        FireProjectile();
                    else
                        _fireDelayWait -= Time.deltaTime;
                }
            }
            else
                _fireDelayWait = FireDelay;
        }
    }

    public virtual void LateUpdate()
    {
        if (!isPlaying)
            return;

        if (isPlaying && !isSocking)
        {
            velocity.x = _direction.x * moveSpeed;
        }

        velocity.y += -gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime, false);

        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;
    }

    public void SetForce(float x, float y)
    {
        velocity = new Vector3(x, y, 0);
    }

    private void FireProjectile()
    {
        if (_fireIn > 0)
            return;

        _fireIn = fireRate;
        var _projectile = (Projectile)Instantiate(projectile, PointSpawn.position, Quaternion.identity);
        _audioSource.PlayOneShot(FireSound);
        EnemyAnim.PlayRangeAttackAnim();
        _projectile.Initialize(gameObject, _direction, Vector2.zero);
    }


    /// <summary>
    /// Takes the damage.
    /// </summary>
    /// <param name="damage">Damage.</param>
    /// <param name="instigator">Instigator.</param>
    public void TakeDamage(float damage, Vector2 force, GameObject instigator)
    {
        if (isDead)
            return;

        pushForce = force;

        if (HurtEffect != null)
            Instantiate(HurtEffect, instigator.transform.position, Quaternion.identity);

        if (healthType == HealthType.HitToKill)
        {
            currentHitLeft--;
            if (currentHitLeft <= 0)
            {
                isDead = true;
            }
        }
        else if (healthType == HealthType.HealthAmount)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                isDead = true;
            }
        }

        //if (instigator.GetComponent<Block>() != null)
        //    isDead = true;

        HitEvent();

    }

    protected virtual void HitEvent()
    {
        _audioSource.PlayOneShot(hurtSound);
        if (HurtEffect != null)
            Instantiate(HurtEffect, transform.position, transform.rotation);

        StopAllCoroutines();
        StartCoroutine(PushBack(0.35f));
    }


    protected virtual void Dead()
    {

        isPlaying = false;

        //		transform.localScale = new Vector3 (1, -1, 1);	//fall

        StopAllCoroutines();
        _audioSource.PlayOneShot(deadSound);
        EnemyAnim.PlayDeathAnim();
        if (pointToGivePlayer != 0)
        {
            AllServices.Instance.GetService<LevelManager>().Point += pointToGivePlayer;
            AllServices.Instance.GetService<FactoryFloatText>().BuildFloatingText(transform.position, "+" + pointToGivePlayer, Color.yellow);
        }

        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        if (spawnItemWhenDead != null)
            Instantiate(spawnItemWhenDead, PointSpawn.position, PointSpawn.rotation);

        //turn off all colliders if the enemy have
        var boxCo = GetComponents<BoxCollider2D>();
        foreach (var box in boxCo)
        {
            box.enabled = false;
        }
        var CirCo = GetComponents<CircleCollider2D>();
        foreach (var cir in CirCo)
        {
            cir.enabled = false;
        }
        StartCoroutine(DeactivateCoroutine());
    }

    public void OnPlayerRespawnInThisCheckPoint(CheckPoint checkpoint, Player player)
    {

        currentHealth = health;
        currentHitLeft = maxHitToKill;
        transform.position = _startPosition;
        transform.localScale = _startScale;
        _direction = Vector2.left;
        velocity = Vector3.zero;
        isPlaying = true;
        isDead = false;
        isSocking = false;
        gameObject.SetActive(true);

        //turn on all colliders if the enemy have
        var boxCo = GetComponents<BoxCollider2D>();
        foreach (var box in boxCo)
        {
            box.enabled = true;
        }
        var CirCo = GetComponents<CircleCollider2D>();
        foreach (var cir in CirCo)
        {
            cir.enabled = true;
        }
        EnemyAnim.BackToWalkAnim();
    }

    public IEnumerator PushBack(float delay)
    {
        isPlaying = false;
        LevelManager levelManager = AllServices.Instance.GetService<LevelManager>();
        SetForce(levelManager.Player.transform.localScale.x * pushForce.x, pushForce.y);

        yield return new WaitForSeconds(delay);
        SetForce(0, 0);

        if (isDead)
            Dead();
        else
            isPlaying = true;
    }

    public void OnDrawGizmosSelected()
    {
        if (isUseProjectile)
        {
            Gizmos.color = Color.blue;
            if (_direction.magnitude != 0)
                Gizmos.DrawRay(PointSpawn.position, _direction * detectDistance);
            else
                Gizmos.DrawRay(PointSpawn.position, Vector2.left * detectDistance);
        }
    }

    private IEnumerator DeactivateCoroutine()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
