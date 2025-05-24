using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public int hp = 100;
    public int damageToGoblin = 50;

    Vector2 movementInput;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    Animator animator;
    public SpriteRenderer spriteRenderer;

    bool canMove = true;

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
    private GameObject dashEffectInstance;

    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime = -Mathf.Infinity;

    [Header("Death Settings")]
    public string deathSceneName = "RespawnToHub";


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        //Movement Handling
        if (canMove && !isDashing) {

            if (movementInput != Vector2.zero) {
                bool success = TryMove(movementInput);
                
                if (!success) {
                    success = TryMove(new Vector2(movementInput.x, 0));
                }

                if (!success) {
                    success = TryMove(new Vector2(0, movementInput.y));
                }

                animator.SetBool("isMoving", success);
            } else {
                animator.SetBool("isMoving", false);
            }

            //Flip direction of sprite based on direction
            if (movementInput.x < 0) spriteRenderer.flipX = true;
            else if (movementInput.x > 0) spriteRenderer.flipX = false;

        }

    }

    //Movement Handling
    private bool TryMove(Vector2 direction) {

        if (direction != Vector2.zero) {

            //Raycast collisions
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset
            );

            if (count == 0) {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }

        } else {
            //Can't move if there's no direction to move in
            return false;
        }
    }

    //Movement Handling
    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnDash()
    {
        if (Time.time >= lastDashTime + dashCooldown && movementInput != Vector2.zero)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        dashTime = dashDuration;
        lastDashTime = Time.time;

        Vector2 dashDirection = movementInput.normalized;

        // Spawn a new effect for each dash
        if (dashEffectPrefab != null)
        {
            GameObject effect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);

            float offsetX = spriteRenderer.flipX ? 0.3f : -0.3f;
            effect.transform.localPosition = new Vector3(offsetX, 0.13f, 0);

            // Flip sprite if needed
            SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = spriteRenderer.flipX;

            // Automatically destroy after duration (match animation length)
            Destroy(effect, 0.4f); // Adjust this if your dash animation is longer
        }

        // Dash movement with wall collision
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
                break; // wall hit
            }

            dashTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    public void TakeDamage(int amount)
    {
        if (hp <= 0) return; // Already dead

        hp -= amount;

        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }
    public void Die()
    {
        if (canMove) // prevent multiple calls
        {
            canMove = false;
            animator.SetTrigger("Die"); // Play death animation

            // Call UIManager to show death and respawn screens + load scene
            UIManager.Instance.ShowDeathAndRespawn(deathSceneName);
        }
    }




    //Attack Handling
    void OnFire()
    {
        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            animator.SetTrigger("attack");
            lastAttackTime = Time.time;
            isAttacking = true;
        }
    }


    public void SwordAttack() {
        LockMovement();

        if (spriteRenderer.flipX == true) attackHitbox.AttackLeft();
        else attackHitbox.AttackRight();
    }

    public void EndSwordAttack()
    {
        UnlockMovement();
        attackHitbox.StopAttack();
        isAttacking = false;
    }


    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }
    //End of Attack Handling

}
