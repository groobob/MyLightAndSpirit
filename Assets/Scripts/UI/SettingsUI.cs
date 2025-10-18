using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button backButton;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider brightnessSlider;
    
    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            Hide();
        });
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
