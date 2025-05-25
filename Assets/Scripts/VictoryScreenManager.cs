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
        // Hide Player HP bar
        if (playerHPBar != null)
            playerHPBar.SetActive(false);

        // Freeze time
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

        // Show victory text
        victoryText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(victoryTextDuration);
        victoryText.gameObject.SetActive(false);

        // Delay before thank you
        yield return new WaitForSecondsRealtime(thankYouDelay);

        // Show thank you text and main menu button
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
