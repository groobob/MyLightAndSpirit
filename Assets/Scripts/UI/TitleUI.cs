using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button startButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;
    [SerializeField] GameObject settingsMenu;

    private void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            SceneLoader.Instance.LoadNextScene();
        });
        settingsButton.onClick.AddListener(() =>
        {
            settingsMenu.GetComponent<SettingsUI>().Show();
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
