using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    [Header("AOE Attack Settings")]
    public GameObject aoePrefab;
    public int totalAOECount = 5;
    public float attackCooldown = 8f;
    public float attackDelay = 0.5f;

    [Header("Boss HP UI")]
    public BossHPBar bossHPBar;

    [Header("Health Settings")]
    public int maxHealth = 200;
    public int currentHealth;

    [Header("Detection Settings")]
    public float detectionRadius = 8f;
    private Transform player;
    private bool playerInRange = false;

    [Header("Death Settings")]
    public GameObject deathEffectPrefab;
    public float destroyDelay = 2f;

    [Header("Map Bounds")]
    public Vector2 mapMinBounds; // Set via Inspector or calculated in Start
    public Vector2 mapMaxBounds;

    [Header("Enemy Spawning")]
    public GameObject goblinPrefab;
    public GameObject skelichPrefab;
    public int goblinsToSpawn = 3;
    public int skelichToSpawn = 2;

    private bool hasSpawned75 = false;
    private bool hasSpawned50 = false;
    private bool hasSpawned25 = false;

    private Animator animator;
    private bool isAttacking = false;
    private bool isDead = false;
    private float lastAttackTime = -Mathf.Infinity;
    public VictoryScreenManager victoryScreenManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        UpdateHealthBar();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Set map bounds here or assign manually in Inspector
        Vector2 mapCenter = new Vector2(0.001600027f, 0.04412293f);
        Vector2 mapExtents = new Vector2(3.559153f, 2.065767f) * 0.8f;

        mapMinBounds = mapCenter - mapExtents;
        mapMaxBounds = mapCenter + mapExtents;
    }

    void Update()
    {
        if (isDead || player == null) return;

        playerInRange = Vector2.Distance(transform.position, player.position) <= detectionRadius;

        float hpPercent = (float)currentHealth / maxHealth;

        if (!hasSpawned75 && hpPercent <= 0.75f)
        {
            SpawnEnemies(goblinsToSpawn, skelichToSpawn);
            hasSpawned75 = true;
        }
        if (!hasSpawned50 && hpPercent <= 0.50f)
        {
            SpawnEnemies(goblinsToSpawn, skelichToSpawn);
            hasSpawned50 = true;
        }
        if (!hasSpawned25 && hpPercent <= 0.25f)
        {
            SpawnEnemies(goblinsToSpawn, skelichToSpawn);
            hasSpawned25 = true;
        }

        if (playerInRange && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        animator.SetBool("IsAttacking", isAttacking || playerInRange);
    }

    public void UpdateHealthBar()
    {
        if (bossHPBar != null)
        {
            float normalizedHealth = (float)currentHealth / maxHealth;
            bossHPBar.SetHealth(normalizedHealth);
        }
    }


    void SpawnEnemies(int goblinCount, int skelichCount)
    {
        for (int i = 0; i < goblinCount; i++)
        {
            Vector2 spawnPos = GetRandomPointInMapBounds();
            Instantiate(goblinPrefab, spawnPos, Quaternion.identity);
        }
        for (int i = 0; i < skelichCount; i++)
        {
            Vector2 spawnPos = GetRandomPointInMapBounds();
            Instantiate(skelichPrefab, spawnPos, Quaternion.identity);
        }
    }

    Vector2 GetRandomPointInMapBounds()
    {
        float x = Random.Range(mapMinBounds.x, mapMaxBounds.x);
        float y = Random.Range(mapMinBounds.y, mapMaxBounds.y);
        return new Vector2(x, y);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(attackDelay);

        // AOE directly on the player
        Instantiate(aoePrefab, player.position, Quaternion.identity);

        for (int i = 1; i < totalAOECount; i++)
        {
            Vector2 randomPos = GetRandomPointInFrontArc(transform.position, detectionRadius);
            Instantiate(aoePrefab, randomPos, Quaternion.identity);
        }

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    Vector2 GetRandomPointInFrontArc(Vector2 center, float radius)
    {
        float angle = Random.Range(-60f, 60f) * Mathf.Deg2Rad;
        Vector2 forward = Vector2.down;

        Vector2 direction = new Vector2(
            forward.x * Mathf.Cos(angle) - forward.y * Mathf.Sin(angle),
            forward.x * Mathf.Sin(angle) + forward.y * Mathf.Cos(angle)
        );

        float distance = Random.Range(0.3f * radius, radius);
        Vector2 rawPos = center + direction.normalized * distance;

        float clampedX = Mathf.Clamp(rawPos.x, mapMinBounds.x, mapMaxBounds.x);
        float clampedY = Mathf.Clamp(rawPos.y, mapMinBounds.y, mapMaxBounds.y);

        return new Vector2(clampedX, clampedY);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.onhitSFX, 1.5f);
        currentHealth -= amount;
        UpdateHealthBar();
        Debug.Log("Boss took damage: " + amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.bossDeathSFX, 1.5f);
        StopAllCoroutines();

        animator.SetBool("IsAttacking", false);
        animator.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Destroy the effect after 2 seconds
        }

        if (victoryScreenManager != null)
        {
            victoryScreenManager.TriggerVictorySequence();
        }

        Destroy(gameObject, destroyDelay);
    }


    void OnDrawGizmosSelected()
    {
        // Draw detection half-circle
        Gizmos.color = Color.red;
        int segments = 32;
        float angleStep = 180f / segments;
        float startAngle = -90f;
        Vector3 origin = transform.position;
        Vector3 forward = Vector3.down;

        Vector3 lastPoint = origin + Quaternion.Euler(0, 0, startAngle) * forward * detectionRadius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 nextPoint = origin + Quaternion.Euler(0, 0, angle) * forward * detectionRadius;
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }

        // Draw safe spawn rectangle
        Gizmos.color = Color.green;

        Vector2 mapCenter = new Vector2(0.001600027f, 0.04412293f);
        Vector2 mapExtents = new Vector2(3.559153f, 2.065767f) * 0.8f;

        Vector2 minBounds = mapCenter - mapExtents;
        Vector2 maxBounds = mapCenter + mapExtents;

        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0f);
        Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0f);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0f);
        Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0f);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }

}
