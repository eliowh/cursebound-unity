using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "CellRoom"; // Change to your actual gameplay scene name

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();

        // This will only work in a built version of the game
        // In the editor, use UnityEditor.EditorApplication.isPlaying = false;
    }
}
