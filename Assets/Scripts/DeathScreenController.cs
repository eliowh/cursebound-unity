using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScreenController : MonoBehaviour
{
    public GameObject DeathScreen;

    public void ShowDeathScreen()
    {
        DeathScreen.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void HideDeathScreen()
    {
        DeathScreen.SetActive(false);
        Time.timeScale = 1f;
    }
}

