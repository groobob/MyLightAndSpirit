using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnPause += UIManager_OnPause;
        GameManager.Instance.OnUnpause += UIManager_OnUnpause;
    }

    private void UIManager_OnPause(object sender, System.EventArgs e)
    {
        pauseMenu.GetComponent<PauseUI>().Show();
    }

    private void UIManager_OnUnpause(object sender, System.EventArgs e)
    {
        pauseMenu.GetComponent<PauseUI>().Hide();
        pauseMenu.GetComponent<PauseUI>().settingsMenu.SetActive(false);
    }
}
