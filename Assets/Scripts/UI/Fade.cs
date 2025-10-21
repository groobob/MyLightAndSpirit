using UnityEngine;

public class Fade : MonoBehaviour
{
    [SerializeField] float fadeTime;
    [SerializeField] float fadeDelay;
    bool fading;
    float timeElapsed;

    private void Update()
    {
        if(fading)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed > fadeTime) Destroy(gameObject);
            Color currentColor = gameObject.GetComponent<SpriteRenderer>().color;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(currentColor.r, currentColor.g, currentColor.b, (fadeTime - timeElapsed) / fadeTime);
        }
        else
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed > fadeDelay)
            {
                fading = true;
                timeElapsed = 0;
            }
        }
    }
}
