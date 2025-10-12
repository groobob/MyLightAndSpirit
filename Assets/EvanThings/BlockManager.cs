using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    [SerializeField] private GameObject shadowWallPrefab;
    public GameObject getShadowWallPrefab()
    {
        return shadowWallPrefab;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}