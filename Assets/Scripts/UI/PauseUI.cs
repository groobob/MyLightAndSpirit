using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitToTitleButton;
    [SerializeField] GameObject settingsMenu;

    [Header("Audio")]
    private int buttonClickSoundID = 6;

    private void Start()
    {
        GameManager.Instance.OnUnpause += PauseUI_OnUnpause;

        resumeButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            GameManager.Instance.Unpause();
            Hide();
        });
        settingsButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            settingsMenu.GetComponent<SettingsUI>().Show();
        });
        quitToTitleButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            SceneLoader.Instance.LoadStartScene();
        });
    }

    private void PauseUI_OnUnpause(object sender, System.EventArgs e)
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance.PlayAudio(buttonClickSoundID, AudioSourceType.UIButtonPress);
    }
}
