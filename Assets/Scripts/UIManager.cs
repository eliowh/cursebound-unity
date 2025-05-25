using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject DeathScreen;      // Assign in inspector
    public GameObject RespawnScreenUI;  // Assign in inspector

    public UnityEngine.UI.Slider hpSlider; // HP bar

    [SerializeField] Animator sceneTransition;

    private string sceneToLoad;
    private GameObject vignetteInstance;

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
        
        // Reassign UI references
        if (DeathScreen == null)
            DeathScreen = GameObject.FindWithTag("DeathScreen");

        if (RespawnScreenUI == null)
            RespawnScreenUI = GameObject.FindWithTag("RespawnScreen");

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
