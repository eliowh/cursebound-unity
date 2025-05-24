using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject DeathScreen;      // Assign in inspector
    public GameObject RespawnScreenUI;  // Assign in inspector

    public GameObject hpUIContainer; // Assign in Inspector
    public UnityEngine.UI.Slider hpSlider; // HP bar
    public UnityEngine.UI.Text hpText;

    [SerializeField] Animator sceneTransition;

    private string sceneToLoad;

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
        Time.timeScale = 0f; // Pause AFTER animation plays

        if (DeathScreen != null)
            DeathScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(2.5f); // Show death UI

        if (DeathScreen != null)
            DeathScreen.SetActive(false);

        if (RespawnScreenUI != null)
            RespawnScreenUI.SetActive(true);

        yield return new WaitForSecondsRealtime(1.0f); // Delay before switching scene

        Time.timeScale = 1f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        yield return new WaitForSecondsRealtime(1.0f); // Optional delay after loading

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

        if (hpSlider == null)
            hpSlider = GameObject.FindWithTag("HPBar")?.GetComponent<UnityEngine.UI.Slider>();
    }


    IEnumerator LoadSceneWithTransition()
    {
        // Play fade out animation
        sceneTransition.SetTrigger("End");

        // Wait for the animation length before continuing
        // Adjust this time to your animation length or better use animation events
        yield return new WaitForSecondsRealtime(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Play fade in animation
        sceneTransition.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(1f); // Wait for fade-in animation to finish
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadRoutine(sceneName));
    }

    private IEnumerator FadeAndLoadRoutine(string sceneName)
    {
        // Trigger fade out animation on the fade UI
        sceneTransition.SetTrigger("End");

        // Wait for animation duration
        yield return new WaitForSecondsRealtime(1f); // match your animation length

        // Load scene async
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // Trigger fade in animation
        sceneTransition.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(1f); // optional fade in delay
    }

    public void UpdateHP(int currentHP, int maxHP)
    {
        if (hpSlider != null)
            hpSlider.value = (float)currentHP / maxHP;

        if (hpText != null)
            hpText.text = currentHP + " / " + maxHP;
    }


}
