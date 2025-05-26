using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject DeathScreen;      // Assign in inspector
    public GameObject RespawnScreenUI;  // Assign in inspector
    public GameObject pauseMenuUI;

    public UnityEngine.UI.Slider hpSlider; // HP bar

    [SerializeField] Animator sceneTransition;

    private string sceneToLoad;
    private GameObject vignetteInstance;
    private bool isPaused = false;

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
        if (vignetteInstance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("GlobalVignette"); // Must match name
            if (prefab != null)
            {
                vignetteInstance = Instantiate(prefab);
                DontDestroyOnLoad(vignetteInstance);
            }
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)) // For desktop; on mobile you can call PauseToggle() from a button
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Replace with your actual main menu scene name
    }

    public void ShowDeathAndRespawn(string sceneName)
    {
        StartCoroutine(DeathRespawnSequence(sceneName));
    }

    public IEnumerator DeathRespawnSequence(string sceneName)
    {
        Time.timeScale = 0f;

        if (DeathScreen != null)
            DeathScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(2.5f);

        if (DeathScreen != null)
            DeathScreen.SetActive(false);

        if (RespawnScreenUI != null)
            RespawnScreenUI.SetActive(true);

        yield return new WaitForSecondsRealtime(1.0f);

        Time.timeScale = 1f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        yield return new WaitForSecondsRealtime(1.0f);

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
        EventSystem[] eventSystems = GameObject.FindObjectsOfType<EventSystem>();
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject); // keep the first, remove others
            }
        }
        // Reassign UI references
        if (DeathScreen == null)
            DeathScreen = GameObject.FindWithTag("DeathScreen");

        if (RespawnScreenUI == null)
            RespawnScreenUI = GameObject.FindWithTag("RespawnScreen");

        if (pauseMenuUI == null)
            pauseMenuUI = GameObject.FindWithTag("PauseMenu");

        if (hpSlider == null)
            hpSlider = GameObject.FindWithTag("HP Bar")?.GetComponent<UnityEngine.UI.Slider>();

    }


    IEnumerator LoadSceneWithTransition()
    {
        sceneTransition.SetTrigger("End");
        yield return new WaitForSecondsRealtime(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        sceneTransition.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(1f);
    }

    public void HideHPBar()
    {
        if (hpSlider != null)
            hpSlider.gameObject.SetActive(false);
    }

    public void ShowHPBar()
    {
        if (hpSlider != null)
            hpSlider.gameObject.SetActive(true);
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadRoutine(sceneName));
    }

    private IEnumerator FadeAndLoadRoutine(string sceneName)
    {
        sceneTransition.SetTrigger("End");
        yield return new WaitForSecondsRealtime(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        sceneTransition.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(1f);
    }

    public void UpdateHP(int currentHP, int maxHP)
    {
        if (hpSlider != null)
            hpSlider.value = (float)currentHP / maxHP;
    }
}
