using UnityEngine;

public class GoblinScript : MonoBehaviour
{
    public float moveSpeed = .7f;
    public float chaseRange = 1f;       // Add this for chase range limit
    public float attackRange = .5f;
    public float attackCooldown = 2f;
    public GameObject staffVisual;

    public float knockbackForce = 3f;
    public float knockbackDuration = 0.2f;

    [Header("Projectile Settings")]
    public float projectileRange = 3f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileCooldown = 2f;

    private float lastProjectileTime;

    private Transform player;
    private PlayerController playerController;
    private Animator animator;
    private Rigidbody2D rb;

    private float lastAttackTime;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private bool isDead = false;

    [Header("Delayed Strike Settings")]
    public GameObject delayedStrikePrefab;
    public float delayedStrikeCooldown = 5f;
    private float lastDelayedStrikeTime;

    [Header("Patrol Settings")]
    private int patrolDirection = 1;

    public int hp = 100;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
{
    if (isDead || player == null || playerController.hp <= 0) return;

    if (isKnockedBack)
    {
        knockbackTimer -= Time.deltaTime;
        if (knockbackTimer <= 0f)
        {
            isKnockedBack = false;
            rb.velocity = Vector2.zero;
        }
        return;
    }

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    if (distanceToPlayer <= chaseRange)
    {
        // Chase logic
        ChasePlayer();
        StopShooting(); // Stop projectiles while chasing
        SetStaffVisible(false);
    }
    else if (distanceToPlayer <= projectileRange)
    {
        // Stop moving and shoot
        animator.SetBool("isMoving", false);
        ShootAtPlayer();
        CastDelayedStrike();
        SetStaffVisible(true);
    }
    else
    {
        StopShooting();
        SetStaffVisible(false);
        Patrol();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Goblin"))
        {
            patrolDirection *= -1;
        }
    }
    void Patrol()
    {
        // Move left or right
        Vector2 move = new Vector2(patrolDirection * moveSpeed, 0);
        transform.position += (Vector3)(move * Time.deltaTime);

        // Animate movement and set direction parameters (like chase)
        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", patrolDirection);
        animator.SetFloat("moveY", 0);
    }

    private void SetStaffVisible(bool isVisible)
    {
        if (staffVisual != null)
        {
            staffVisual.SetActive(isVisible);
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);

        // Attack if close enough
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }

    void ShootAtPlayer()
    {
        if (Time.time >= lastProjectileTime + projectileCooldown)
        {
            ShootProjectile();
            lastProjectileTime = Time.time;
        }
    }

    void StopShooting()
    {
        // Optional: reset projectile timer or keep it paused
        lastProjectileTime = Time.time; // Prevents shooting immediately after exiting chase range
    }


    private void AttackPlayer()
    {
        if (playerController != null)
        {
            playerController.TakeDamage(15);
            Debug.Log("Goblin attacked! Player HP: " + playerController.hp);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;
        Debug.Log("Goblin took " + damage + " damage! Remaining HP: " + hp);

        ApplyKnockback();

        if (hp <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback()
    {
        if (player == null) return;

        Vector2 knockDirection = (transform.position - player.position).normalized;
        rb.velocity = knockDirection * knockbackForce;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }
    private void ShootProjectile()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null) return;

        // Calculate direction
        Vector2 direction = (player.position - projectileSpawnPoint.position).normalized;

        // Calculate rotation angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate and rotate the projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.Euler(0, 0, angle));

        // Apply velocity
        Rigidbody2D rbProjectile = projectile.GetComponent<Rigidbody2D>();
        rbProjectile.velocity = direction * 2f;
    }

    void CastDelayedStrike()
    {
        if (Time.time >= lastDelayedStrikeTime + delayedStrikeCooldown && delayedStrikePrefab != null && player != null)
        {
            Instantiate(delayedStrikePrefab, player.position, Quaternion.identity);
            lastDelayedStrikeTime = Time.time;
        }
    }


    private void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        animator.SetTrigger("Die");

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        // Notify GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.GoblinDefeated();
        }

        Destroy(gameObject, 1.5f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, projectileRange);
    }

}