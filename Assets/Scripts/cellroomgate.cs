using UnityEngine;

public class cellroomgate : MonoBehaviour
{

    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DoorOpens()
    {
        animator.SetBool("cellGateOpens", true);
    }

    public void DoorCloses()
    {
        animator.SetBool("cellGateOpens", false);
    }
}
