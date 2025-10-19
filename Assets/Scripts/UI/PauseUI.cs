using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitToTitleButton;
    [SerializeField] GameObject settingsMenu;

    private void Start()
    {
        GameManager.Instance.OnUnpause += PauseUI_OnUnpause;

        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.Unpause();
            Hide();
        });
        settingsButton.onClick.AddListener(() =>
        {
            settingsMenu.GetComponent<SettingsUI>().Show();
        });
        quitToTitleButton.onClick.AddListener(() =>
        {
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
}
