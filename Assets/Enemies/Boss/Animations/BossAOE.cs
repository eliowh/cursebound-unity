using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAOE : MonoBehaviour
{
    private Animator animator;
    private bool isActive = false; // Only true during loop
    public int damageAmount = 20;
    public float damageInterval = 1f;

    private Dictionary<GameObject, float> lastDamageTime = new();
    private Collider2D aoeCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        aoeCollider = GetComponent<Collider2D>();

        aoeCollider.enabled = false; // Disable during summon
        StartCoroutine(SummonThenLoopThenDisappear());
    }

    IEnumerator SummonThenLoopThenDisappear()
    {
        // Wait for summon animation to finish (e.g., 1.0s)
        yield return new WaitForSeconds(1.0f);

        // Enable loop phase and trigger
        isActive = true;
        aoeCollider.enabled = true;

        // Persist during loop phase
        yield return new WaitForSeconds(6f);

        // Disable trigger before disappear
        isActive = false;
        aoeCollider.enabled = false;

        // Trigger disappear animation
        animator.SetTrigger("Disappear");
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            float lastTime;
            lastDamageTime.TryGetValue(player, out lastTime);

            if (Time.time - lastTime >= damageInterval)
            {
                lastDamageTime[player] = Time.time;

                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.TakeDamage(damageAmount);
                }
            }
        }
    }

    // Called at the end of Disappear animation via Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
