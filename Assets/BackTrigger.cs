using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackTrigger : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered exit trigger, loading next scene...");
            UIManager.Instance.FadeAndLoadScene(sceneToLoad);
        }
    }
}
