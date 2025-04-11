using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Components

    Rigidbody2D rb;
    SpriteRenderer sr;

    // attributes

    float movespd = 1;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Store Vertical & Horizontal Axis Input
        float vAxis = Input.GetAxis("Vertical");
        float hAxis = Input.GetAxis("Horizontal");

        //Set Velocity of Rigidbody
        rb.velocity = new Vector2(hAxis * movespd, vAxis * movespd);

        if (hAxis < 0) sr.flipX = true;
        else if (hAxis > 0) sr.flipX = false;
    }
}
