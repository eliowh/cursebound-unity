using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

using UnityEngine;

public class GoblinProjectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("Player"))
        {
            // Optional: apply damage here if you want
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(10); // Or however much damage you want
                Debug.Log("Player hit by projectile! HP: " + player.hp);
            }

            Destroy(gameObject);
        }
    }
}

