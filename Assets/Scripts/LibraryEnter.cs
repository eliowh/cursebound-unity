using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LibraryEnter : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision) {
        //Next area handling
        if (collision.gameObject.CompareTag("PlayerTrigger")) {
            UIManager.Instance.FadeAndLoadScene(sceneToLoad);
        }
    }
}
