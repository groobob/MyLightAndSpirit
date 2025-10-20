using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public TMP_Text speakerNameText;
    public Image portraitImage;

    [Header("Settings")]
    public float typeSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private string currentLine;

    public void Show()
    {
        dialogueBox.SetActive(true);
    }

    public void Hide()
    {
        dialogueBox.SetActive(false);
    }

    public void SetSpeaker(string name, Sprite portrait)
    {
        // Handle name
        if (!string.IsNullOrEmpty(name))
        {
            speakerNameText.gameObject.SetActive(true);
            speakerNameText.text = name;
        }
        else
        {
            speakerNameText.gameObject.SetActive(false);
        }

        // Handle portrait
        if (portrait != null)
        {
            portraitImage.gameObject.SetActive(true);
            portraitImage.sprite = portrait;
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }
    }

    public void TypeLine(string line, System.Action onComplete)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line, onComplete));
    }

    private IEnumerator TypeText(string line, System.Action onComplete)
    {
        currentLine = line;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        onComplete?.Invoke();
    }

    public void FinishTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = currentLine;
    }
}
