using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Common")]
    public GameObject exitGate;
    public GameObject exitTrigger;

    [Header("Maze1 Settings")]
    public int maze1GoblinsToDefeat = 6;

    [Header("Maze2 Settings")]
    public int skelichToDefeat = 5;
    public int goblinsToDefeat = 3;
    public GameObject mimicMinibossPrefab;
    public Transform mimicSpawnPoint;

    private int goblinsDefeated = 0;
    private int skelichDefeated = 0;
    private bool mimicSummoned = false;
    private bool mimicDefeated = false;

    private enum GameScene { Maze1, Maze2, Unknown }
    private GameScene currentScene = GameScene.Unknown;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // Optional if you want to keep this manager between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Detect current scene to set logic
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Maze1")
        {
            currentScene = GameScene.Maze1;
        }
        else if (sceneName == "Maze2")
        {
            currentScene = GameScene.Maze2;
        }
        else
        {
            currentScene = GameScene.Unknown;
        }
    }

    public void GoblinDefeated()
    {
        if (currentScene == GameScene.Maze1)
        {
            goblinsDefeated++;
            Debug.Log($"Maze1 Goblins defeated: {goblinsDefeated}/{maze1GoblinsToDefeat}");
            if (goblinsDefeated >= maze1GoblinsToDefeat)
            {
                OpenExit();
            }
        }
        else if (currentScene == GameScene.Maze2)
        {
            goblinsDefeated++;
            Debug.Log($"Maze2 Goblins defeated: {goblinsDefeated}/{goblinsToDefeat}");
            CheckMaze2Progress();
        }
    }

    public void SkelichDefeated()
    {
        if (currentScene == GameScene.Maze2)
        {
            skelichDefeated++;
            Debug.Log($"Skelich defeated: {skelichDefeated}/{skelichToDefeat}");
            CheckMaze2Progress();
        }
    }

    public void MimicDefeated()
    {
        if (currentScene == GameScene.Maze2 && mimicSummoned)
        {
            mimicDefeated = true;
            Debug.Log("Mimic miniboss defeated!");
            OpenExit();
        }
    }

    private void CheckMaze2Progress()
    {
        if (!mimicSummoned && goblinsDefeated >= goblinsToDefeat && skelichDefeated >= skelichToDefeat)
        {
            SummonMimic();
        }
    }

    private void SummonMimic()
    {
        if (mimicMinibossPrefab != null && mimicSpawnPoint != null)
        {
            Instantiate(mimicMinibossPrefab, mimicSpawnPoint.position, Quaternion.identity);
            mimicSummoned = true;
            Debug.Log("Mimic miniboss summoned!");
        }
    }

    void OpenExit()
    {
        if (exitGate != null)
        {
            exitGate.SetActive(false);
            Debug.Log("Exit is now open!");
        }

        if (exitTrigger != null)
        {
            exitTrigger.SetActive(true);
            Debug.Log("Exit trigger is now visible!");
        }
    }
}
