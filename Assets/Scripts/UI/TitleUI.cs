using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button startButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;
    [SerializeField] GameObject settingsMenu;

    [Header("Audio")]
    private int buttonClickSoundID = 6;

    private void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            SceneLoader.Instance.LoadNextScene();
        });
        settingsButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            settingsMenu.GetComponent<SettingsUI>().Show();
        });
        quitButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            Application.Quit();
        });
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance.PlayAudio(buttonClickSoundID, AudioSourceType.UIButtonPress);
    }
}
