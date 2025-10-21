using UnityEngine;

public class ReturnToTitle : MonoBehaviour
{
    [SerializeField] float callTime;
    float timeElapsed;

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > callTime)
        {
            SceneLoader.Instance.LoadStartScene();
        }
    }
}
