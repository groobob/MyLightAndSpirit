using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    [SerializeField] private GameObject shadowWallPrefab;
    [SerializeField] private GameObject switchPrefab;
    public GameObject getShadowWallPrefab()
    {
        return shadowWallPrefab;
    }

    public GameObject getSwitchPrefab()
    {
        return switchPrefab;
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