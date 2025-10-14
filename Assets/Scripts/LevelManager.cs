using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Singleton
    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
