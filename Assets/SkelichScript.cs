using System.Collections;
using UnityEngine;

public class SkelichScript : MonoBehaviour
{
    [Header("Stats")]
    public int hp = 100;
    public float patrolSpeed = 0.4f;
    public float fleeRange = 1f;
    public float projectileRange = 3f;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float projectileCooldown = 2f;
    public float fleeRangeAttackCooldown = 0.75f; // Faster cooldown when close
    public Transform projectileSpawnPoint;

    [Header("Knockback Settings")]
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.2f;

    [Header("Patrol Random Switch Settings")]
    public float patrolDirectionChangeMinTime = 2f;
    public float patrolDirectionChangeMaxTime = 5f;

    [Header("Patrol Random Stop Settings")]
    public float stopDurationMin = 1f;
    public float stopDurationMax = 3f;

    [Header("Death Effect")]
    public GameObject deathParticlesPrefab;

    private bool isStopped = false;
    private float stopTimer = 0f;

    private float lastProjectileTime = 0f;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private int patrolDirection = 1;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private bool isDead = false;
    private bool canMove = true;

    private float patrolDirectionChangeTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        patrolDirection = Random.value < 0.5f ? -1 : 1;
        ResetPatrolDirectionChangeTimer();
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                rb.velocity = Vector2.zero;

                // Reset animation states after knockback ends
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= projectileRange)
        {
            AttackPlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
            HandleRandomPatrolDirectionChange();
        }
    }

    void Patrol()
    {
        if (!canMove) return;

        if (isStopped)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);

            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0f)
            {
                isStopped = false;
                ResetPatrolDirectionChangeTimer();
            }
        }
        else
        {
            Vector2 move = new Vector2(patrolDirection * patrolSpeed, rb.velocity.y);
            rb.velocity = move;

            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
            animator.SetFloat("moveX", patrolDirection);

            if (patrolDirectionChangeTimer <= 0f)
            {
                if (Random.value > 0.5f)
                {
                    isStopped = true;
                    stopTimer = Random.Range(stopDurationMin, stopDurationMax);
                }
                else
                {
                    patrolDirection *= -1;
                    ResetPatrolDirectionChangeTimer();
                }
            }
        }
    }

    void HandleRandomPatrolDirectionChange()
    {
        if (!isStopped)
        {
            patrolDirectionChangeTimer -= Time.deltaTime;
        }
    }

    void ResetPatrolDirectionChangeTimer()
    {
        patrolDirectionChangeTimer = Random.Range(patrolDirectionChangeMinTime, patrolDirectionChangeMaxTime);
    }

    void AttackPlayer(float distance)
    {
        if (!canMove) return;

        rb.velocity = Vector2.zero;

        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);

        float currentCooldown = distance <= fleeRange ? fleeRangeAttackCooldown : projectileCooldown;

        if (Time.time >= lastProjectileTime + currentCooldown)
        {
            ShootProjectile();
            lastProjectileTime = Time.time;
        }

        // Face player by setting "moveX" parameter (+1 or -1)
        float directionX = player.position.x - transform.position.x;
        animator.SetFloat("moveX", Mathf.Sign(directionX));
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null || player == null) return;

        Vector2 direction = (player.position - projectileSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.Euler(0, 0, angle));
        Rigidbody2D rbProjectile = projectile.GetComponent<Rigidbody2D>();
        rbProjectile.velocity = direction * 2f;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;
        Debug.Log("Skelich took " + damage + " damage! Remaining HP: " + hp);

        ApplyKnockback();

        if (hp <= 0)
        {
            Die();
        }
    }

    void ApplyKnockback()
    {
        if (player == null) return;

        Vector2 knockDirection = (transform.position - player.position).normalized;
        rb.velocity = knockDirection * knockbackForce;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        canMove = false;

        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
        //animator.SetTrigger("Die");

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        // Flash white on death
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        StartCoroutine(PlayDeathEffect());

        if (GameManager.instance != null)
        {
            GameManager.instance.SkelichDefeated();
        }

        Destroy(gameObject);
    }

    private IEnumerator PlayDeathEffect()
    {
        yield return new WaitForSeconds(0.5f); // Flash duration

        GameObject deathEffect = null;

        if (deathParticlesPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.z = 0f;
            deathEffect = Instantiate(deathParticlesPrefab, spawnPos, Quaternion.identity);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Hide sprite after particles
        }

        yield return new WaitForSeconds(1.0f); // Particle effect duration

        if (deathEffect != null)
        {
            Destroy(deathEffect);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Goblin"))
        {
            patrolDirection *= -1;
            ResetPatrolDirectionChangeTimer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, projectileRange);
    }
}
