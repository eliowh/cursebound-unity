using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryScreenManager : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI victoryText;
    public TextMeshProUGUI thankYouText;
    public Button mainMenuButton;
    public GameObject playerHPBar;
    public string gameSceneName = "MainMenu";

    public float delayBeforeFreeze = 1f;
    public float fadeDuration = 2f;
    public float victoryTextDuration = 2f;
    public float thankYouDelay = 1f;

    void Start()
    {
        // Hide all initially
        victoryText.gameObject.SetActive(false);
        thankYouText.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        fadeImage.color = new Color(0, 0, 0, 0); // fully transparent
    }

    public void TriggerVictorySequence()
    {
        StartCoroutine(VictoryFlow());
    }

    IEnumerator VictoryFlow()
    {
        // Hide Player HP bar through UIManager
        if (UIManager.Instance != null)
            UIManager.Instance.HideHPBar();
        else if (playerHPBar != null)
            playerHPBar.SetActive(false);

        yield return new WaitForSeconds(delayBeforeFreeze);

        Time.timeScale = 0f;

        // Fade in screen using unscaled time
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.raycastTarget = false;

        victoryText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(victoryTextDuration);
        victoryText.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(thankYouDelay);

        thankYouText.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
    }


    public void ReturnToMainMenu()
    {
        Debug.Log("Attempting to load scene: " + gameSceneName);
        Time.timeScale = 1f; // Resume time before switching scenes
        SceneManager.LoadScene(gameSceneName);
    }
}
