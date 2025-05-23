using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int goblinsDefeated = 0;
    public int goblinsToDefeatToOpenExit = 5;
    public GameObject exitGate; // Reference to exit seal object

    void Awake()
    {
        // Singleton pattern for easy access
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void GoblinDefeated()
    {
        goblinsDefeated++;
        Debug.Log("Goblins defeated: " + goblinsDefeated);

        if (goblinsDefeated >= goblinsToDefeatToOpenExit)
        {
            OpenExit();
        }
    }

    void OpenExit()
    {
        if (exitGate != null)
        {
            // Example: disable the seal or open the gate
            exitGate.SetActive(false);
            Debug.Log("Exit is now open!");
        }
    }
}