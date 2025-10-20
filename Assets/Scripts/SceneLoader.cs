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

    public void Start(){
        if (SceneManager.GetActiveScene().name == "TitleScene"){
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayMusic(2, AudioSourceType.IntroMusic);
            }
        } else if (SceneManager.GetActiveScene().name == "GameScene"){
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayMusic(0, AudioSourceType.MainMusic);
            }
        }
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

        // stop music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic(2, AudioSourceType.IntroMusic);
        }
    }

    /**
     * Quits the game application
     */
    public void QuitGame()
    {
        Application.Quit();
        // stop music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic(2, AudioSourceType.MainMusic);
        }
        
    }
}
