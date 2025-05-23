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
    }
}
