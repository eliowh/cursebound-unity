using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextAreaLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger"))  // Use "Player" tag here, not "PlayerTrigger"
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.FadeAndLoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogWarning("UIManager instance not found!");
                SceneManager.LoadScene(sceneToLoad); // fallback direct load
            }
        }
    }

}
