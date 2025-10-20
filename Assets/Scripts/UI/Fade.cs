using UnityEngine;
using TMPro;

public class Fade : MonoBehaviour
{
    [SerializeField] float fadeTime;
    [SerializeField] bool fade;
    [SerializeField] TextMeshProUGUI text;
    float timeElapsed;

    private void Update()
    {
        if (fade)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed > fadeTime) Destroy(gameObject);
            text.color = new Color(text.color.r, text.color.g, text.color.b, (fadeTime - timeElapsed) / fadeTime);
        }
    }
}
