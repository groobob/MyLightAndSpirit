using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.CodeDom.Compiler;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] Sprite[] panels;
    [SerializeField] string[] text;

    [SerializeField] Button nextButton;
    [SerializeField] TextMeshProUGUI buttonText;

    [SerializeField] SpriteRenderer panel;
    [SerializeField] TextMeshProUGUI comicText;
    [SerializeField] bool returnToTitle = false;
    private int index = 0;

    private void Start()
    {
        // Play intro music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusic(2, AudioSourceType.IntroMusic);
        }
        
        buttonText.text = "next";
        GenerateNext();
        nextButton.onClick.AddListener(() =>
        {
            if(index ==  panels.Length)
            {
                if (!returnToTitle) SceneLoader.Instance.LoadNextScene();
                else SceneLoader.Instance.LoadStartScene();
            }
            else GenerateNext();
        });
    }

    public void GenerateNext()
    {
        panel.sprite = panels[index];
        comicText.text = text[index];
        index++;
        if(index == panels.Length)
        {
            buttonText.text = "continue";
        }
    }
}
