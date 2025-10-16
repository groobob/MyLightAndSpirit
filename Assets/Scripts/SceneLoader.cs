using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private void Awake()
    {
        Instance = this;
    }

    /**
     * Loads the next scene in the build index
     */
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /**
     * Loads the start scene (scene at build index 0)
     */
    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    /**
     * Quits the game application
     */
    public void QuitGame()
    {
        Application.Quit();
    }
}
