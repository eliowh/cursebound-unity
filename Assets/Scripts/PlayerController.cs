using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject deathParticlesPrefab;
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public int maxHP = 200;
    public int hp;
    public int damageToGoblin = 50;

    Vector2 movementInput;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    Animator animator;
    public SpriteRenderer spriteRenderer;

    bool canMove = true;
    private bool isDead = false;

    public AttackHitbox attackHitbox;

    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;  // Adjust as needed
    private float lastAttackTime = -Mathf.Infinity;
    private bool isAttacking = false;

    [Header("Dash Settings")]
    public float dashSpeed = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public GameObject dashEffectPrefab;
    private bool isInvincible = false;

    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime = -Mathf.Infinity;

    [Header("Death Settings")]
    public string deathSceneName = "RespawnToHub";

    void Start()
    {
        hp = maxHP;
        UIManager.Instance?.UpdateHP(hp, maxHP);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (canMove && !isDashing)
        {
            if (movementInput != Vector2.zero)
            {
                bool success = TryMove(movementInput);

                if (!success)
                    success = TryMove(new Vector2(movementInput.x, 0));

                if (!success)
                    success = TryMove(new Vector2(0, movementInput.y));

                animator.SetBool("isMoving", success);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

            if (movementInput.x < 0) spriteRenderer.flipX = true;
            else if (movementInput.x > 0) spriteRenderer.flipX = false;
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        if (isDead) return;
        movementInput = movementValue.Get<Vector2>();
    }

    void OnDash()
    {
        if (isDead) return;
        if (Time.time >= lastDashTime + dashCooldown && movementInput != Vector2.zero)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;

        dashTime = dashDuration;
        lastDashTime = Time.time;

        Vector2 dashDirection = movementInput.normalized;

        if (dashEffectPrefab != null)
        {
            GameObject effect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);

            float offsetX = spriteRenderer.flipX ? 0.3f : -0.3f;
            effect.transform.localPosition = new Vector3(offsetX, 0.13f, 0);

            SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = spriteRenderer.flipX;

            Destroy(effect, 0.4f);
        }

        while (dashTime > 0f)
        {
            float dashStep = dashSpeed * Time.fixedDeltaTime;
            int count = rb.Cast(dashDirection, movementFilter, castCollisions, dashStep + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + dashDirection * dashStep);
            }
            else
            {
                break;
            }

            dashTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isInvincible = false;
        isDashing = false;
    }

    public void TakeDamage(int amount)
    {
        if (hp <= 0 || isInvincible || isDead) return;

        hp -= amount;
        if (hp < 0) hp = 0;

        UIManager.Instance?.UpdateHP(hp, maxHP);

        if (hp <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        canMove = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        StartCoroutine(PlayDeathEffect());
    }

    private IEnumerator PlayDeathEffect()
    {
        yield return new WaitForSeconds(0.5f);

        if (deathParticlesPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.z = 0f;
            Instantiate(deathParticlesPrefab, spawnPos, Quaternion.identity);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        yield return new WaitForSeconds(0.5f);

        UIManager.Instance.ShowDeathAndRespawn(deathSceneName);
    }

    void OnFire()
    {
        if (isDead) return;

        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            animator.SetTrigger("attack");
            lastAttackTime = Time.time;
            isAttacking = true;
        }
    }

    public void SwordAttack()
    {
        if (isDead) return;

        LockMovement();

        if (spriteRenderer.flipX)
            attackHitbox.AttackLeft();
        else
            attackHitbox.AttackRight();
    }

    public void EndSwordAttack()
    {
        if (isDead) return;

        UnlockMovement();
        attackHitbox.StopAttack();
        isAttacking = false;
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        if (!isDead)
            canMove = true;
    }
}
