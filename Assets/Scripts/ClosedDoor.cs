using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedDoor : MonoBehaviour
{
    Animator animator;
    BoxCollider2D collider2D;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collider2D = GetComponent<BoxCollider2D>();
    }

    public void DoorOpens() {
        animator.SetBool("canOpen", true);
        collider2D.isTrigger = true;
    }

    public void DoorCloses() {
        animator.SetBool("canOpen", false);
        collider2D.isTrigger = false;
    }
}
