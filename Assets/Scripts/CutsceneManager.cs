using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] Sprite[] panels;
    [SerializeField] string[] text;

    [SerializeField] Button nextButton;
    [SerializeField] TextMeshProUGUI buttonText;

    [SerializeField] SpriteRenderer panel;
    [SerializeField] TextMeshProUGUI comicText;
    [SerializeField] bool returnToTitle = false;

    [SerializeField] float scrollSpeed = 0.03f; // seconds per character

    private int index = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string previousText = "";

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
            // If still typing, skip to full text instantly
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                comicText.text = text[index - 1]; // Show full line
                isTyping = false;
                return;
            }

            // Go to next panel or scene
            if (index == panels.Length)
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

        string newText = text[index];
        index++;

        // Skip animation if same text as previous
        if (newText == previousText)
        {
            comicText.text = newText;
            isTyping = false;
        }
        else
        {
            // Stop any running typing coroutine before starting a new one
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(newText));
        }

        previousText = newText;

        if (index == panels.Length)
        {
            buttonText.text = "continue";
        }
    }

    private IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        comicText.text = "";
        foreach (char c in fullText)
        {
            comicText.text += c;
            yield return new WaitForSeconds(scrollSpeed);
        }
        isTyping = false;
    }
}
