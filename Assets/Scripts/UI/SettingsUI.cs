using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button backButton;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider brightnessSlider;

    [SerializeField] Light2D globalLight;

    [Header("Audio")]
    private int buttonClickSoundID = 6;
    
    private void Start()
    {
        if(PlayerPrefs.HasKey("volume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("volume");
            SoundManager.Instance.SetMasterVolume(PlayerPrefs.GetFloat("volume"));
        }
        if(PlayerPrefs.HasKey("brightness"))
        {
            brightnessSlider.value = PlayerPrefs.GetFloat("brightness");
            if(globalLight) globalLight.intensity = PlayerPrefs.GetFloat("brightness");
        }
        backButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            Hide();
        });
        volumeSlider.onValueChanged.AddListener((float value) =>
        {
            PlayerPrefs.SetFloat("volume", value);
            SoundManager.Instance.SetMasterVolume(value);
        });
        brightnessSlider.onValueChanged.AddListener((float value) =>
        {
            PlayerPrefs.SetFloat("brightness", value);
            if(globalLight) globalLight.intensity = value;
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
