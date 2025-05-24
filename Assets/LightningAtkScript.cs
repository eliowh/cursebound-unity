using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAtkScript : MonoBehaviour
{
    public float damageDelay = 1f; // Time before damage
    public int damage = 30;

    private bool hasDealtDamage = false;
    private float timer = 0f;
    private PlayerController player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= damageDelay && !hasDealtDamage)
        {
            if (player != null && Vector2.Distance(transform.position, player.transform.position) < 0.5f)
            {
                player.TakeDamage(damage);
                Debug.Log("Player hit by lightning strike! HP: " + player.hp);
            }

            hasDealtDamage = true;

            // Optional: destroy with delay to let animation finish
            Destroy(gameObject, 0.5f);
        }
    }
}
