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
    private int index = 0;

    private void Start()
    {
        buttonText.text = "next";
        GenerateNext();
        nextButton.onClick.AddListener(() =>
        {
            if(index ==  panels.Length)
            {
                SceneLoader.Instance.LoadNextScene();
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
