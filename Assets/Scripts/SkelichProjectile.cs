using UnityEngine;

public class SkelichProjectile : MonoBehaviour
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
                player.TakeDamage(30); // Or however much damage you want
                Debug.Log("Player hit by projectile! HP: " + player.hp);
            }

            Destroy(gameObject);
        }
    }
}
