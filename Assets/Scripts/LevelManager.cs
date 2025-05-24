using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    PlayerController playerController;

    public string targetSceneName;
    public GameObject childToActivate;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == targetSceneName)
        {
            if (childToActivate != null)
            {
                childToActivate.SetActive(true);
            }
        }
        else
        {
            if (childToActivate != null)
            {
                childToActivate.SetActive(false);
            }
        }
    }

    //void Update()
    //{
    //    if (playerController.hp <= 0) {
    //        SceneManager.LoadScene(sceneToLoad);
    //    }
    //}
}
