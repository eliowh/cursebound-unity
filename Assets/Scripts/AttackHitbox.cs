using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public Collider2D swordCollider;
    public int swordDamage = 30;

    private void Start()
    {
        swordCollider.enabled = false; // Disable by default
    }

    public void AttackRight()
    {
        swordCollider.enabled = true;
        transform.localScale = new Vector3(1, 1, 1); // Normal facing
    }

    public void AttackLeft()
    {
        swordCollider.enabled = true;
        transform.localScale = new Vector3(-1, 1, 1); // Flip horizontally
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goblin"))
        {
            GoblinScript goblin = other.GetComponent<GoblinScript>();
            if (goblin != null)
            {
                goblin.TakeDamage(swordDamage);
                Debug.Log("Goblin Hit! HP left: " + goblin.hp);
            }
        }

        if (other.CompareTag("Skelich"))
        {
            SkelichScript skelich = other.GetComponent<SkelichScript>();
            if (skelich != null)
            {
                skelich.TakeDamage(swordDamage);
                Debug.Log("Skelich Hit! HP left: " + skelich.hp);
            }
        }

        if (other.CompareTag("Mimic"))
        {
            MimicScript mimic = other.GetComponent<MimicScript>();
            if (mimic != null)
            {
                mimic.TakeDamage(swordDamage);
                Debug.Log("Skelich Hit! HP left: " + mimic.hp);
            }
        }

        if (other.CompareTag("Boss"))
        {
            BossScript boss = other.GetComponent<BossScript>();
            if (boss != null)
            {
                boss.TakeDamage(swordDamage);
                Debug.Log("Boss Hit! HP left: " + boss.currentHealth);
            }
        }

        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            Debug.Log("Enemy projectile destroyed by sword!");
        }
    }
}
