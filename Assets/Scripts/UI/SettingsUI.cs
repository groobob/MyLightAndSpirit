using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button backButton;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider brightnessSlider;

    [Header("Audio")]
    private int buttonClickSoundID = 6;
    
    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
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

    private void PlayButtonSound()
    {
        SoundManager.Instance.PlayAudio(buttonClickSoundID, AudioSourceType.UIButtonPress);
    }
}
