using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision) {
        //Order in Layer handling
        if (collision.gameObject.CompareTag("OpenTrigger")) {
            playerController.spriteRenderer.sortingOrder = 0;
            // Debug.Log("Order in layer: " + playerController.spriteRenderer.sortingOrder);
        }

        if (collision.gameObject.CompareTag("HubOrderTrigger") && 
            playerController.spriteRenderer.sortingOrder == 0  ){

            playerController.spriteRenderer.sortingOrder = -2;

        } else if ( collision.gameObject.CompareTag("HubOrderTrigger") && 
                    playerController.spriteRenderer.sortingOrder == -2 ){
                
                    playerController.spriteRenderer.sortingOrder = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        //Order in Layer handling
        if (collision.gameObject.CompareTag("OpenTrigger")) {
            playerController.spriteRenderer.sortingOrder = 2;
            Debug.Log("Order in layer: " + playerController.spriteRenderer.sortingOrder);
        }

        if (collision.gameObject.CompareTag("HubOrderTrigger") && 
            playerController.spriteRenderer.sortingOrder == 0  ){

            playerController.spriteRenderer.sortingOrder = 0;

        } else if ( collision.gameObject.CompareTag("HubOrderTrigger") && 
                    playerController.spriteRenderer.sortingOrder == -2 ){
                
                    playerController.spriteRenderer.sortingOrder = -2;
        }
    }
}
