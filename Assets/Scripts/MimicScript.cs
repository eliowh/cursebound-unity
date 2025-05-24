using System.Collections;
using UnityEngine;

public class MimicScript : MonoBehaviour
{
    [Header("Combat Settings")]
    public int hp = 300;

    [Header("Attack Settings")]
    public float attackCooldown = 3f;
    public float attackRange = 4f;
    public GameObject lightningPrefab;

    [Header("Detection")]
    public Transform player;
    public LayerMask playerLayer;

    [Header("VFX")]
    public GameObject deathParticlesPrefab;

    [Header("Sprite Settings")]
    public Sprite normalSprite;  // Assign your normal idle sprite here

    [Header("Gizmo")]
    public Color attackRangeColor = Color.cyan;

    private Animator animator;
    private bool canAttack = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start with animator disabled to show normal sprite
        if (animator != null)
            animator.enabled = false;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Ensure normal sprite is set at start
        if (normalSprite != null)
            spriteRenderer.sprite = normalSprite;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (animator != null && !animator.enabled)
                animator.enabled = true;

            if (canAttack)
                StartCoroutine(PerformAttack());
        }
        else
        {
            if (animator != null && animator.enabled)
                animator.enabled = false;

            if (normalSprite != null)
                spriteRenderer.sprite = normalSprite;

            spriteRenderer.enabled = true;
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        if (animator != null && animator.enabled)
            animator.SetTrigger("Aggro"); // Trigger attack animation (looping Aggro)

        yield return new WaitForSeconds(0.5f); // Sync with animation timing

        if (lightningPrefab != null && player != null)
        {
            Vector3 targetPosition = player.position + new Vector3(0f, 0.222f, 0f);
            targetPosition.z = 0f; // 2D friendly
            Instantiate(lightningPrefab, targetPosition, Quaternion.identity);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            StartCoroutine(PlayDeathEffect());
        }
    }

    private IEnumerator PlayDeathEffect()
    {
        yield return new WaitForSeconds(0.2f);

        if (deathParticlesPrefab != null)
        {
            GameObject effect = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.MimicDefeated();
        }

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = attackRangeColor;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
