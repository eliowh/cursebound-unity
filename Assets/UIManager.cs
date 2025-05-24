using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject DeathScreen;      // Assign in inspector
    public GameObject RespawnScreenUI;  // Assign in inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public void ShowDeathAndRespawn(string sceneName)
    {
        StartCoroutine(DeathRespawnSequence(sceneName));
    }

    public IEnumerator DeathRespawnSequence(string sceneName)
    {
        if (DeathScreen != null)
            DeathScreen.SetActive(true); // Show death screen

        yield return new WaitForSecondsRealtime(2.5f);

        if (DeathScreen != null)
            DeathScreen.SetActive(false); // Hide death screen

        if (RespawnScreenUI != null)
            RespawnScreenUI.SetActive(true); // Show respawn screen

        yield return new WaitForSecondsRealtime(1.0f); // Brief delay before load

        // Load the new scene while keeping the respawn UI visible
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Optional: wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Hide respawn UI *after* new scene is loaded
        yield return new WaitForSecondsRealtime(1.0f); // Optional delay after load

        if (RespawnScreenUI != null)
            RespawnScreenUI.SetActive(false);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to find new references to the UI if null
        if (DeathScreen == null)
            DeathScreen = GameObject.FindWithTag("DeathScreen"); // Tag your new canvas' death UI

        if (RespawnScreenUI == null)
            RespawnScreenUI = GameObject.FindWithTag("RespawnScreen");
    }



}
