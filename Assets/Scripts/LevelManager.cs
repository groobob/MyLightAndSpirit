using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Singleton
    public static LevelManager Instance;

    [Header("References")]
    [SerializeField] private GameObject[] levelTilemapObjects;
    [SerializeField] private GameObject gridObject;

    GameObject currentLevel;
    int nextLevelIndex = 0;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //generate first level
        GenerateNextLevel();
    }

    /**
     * Destroys the current level and generates the next level in the list
     */
    public void GenerateNextLevel()
    {
        DestroyLevel();

        currentLevel = Instantiate(levelTilemapObjects[nextLevelIndex], Vector3.zero, Quaternion.identity);
        currentLevel.transform.SetParent(gridObject.transform, true);
        nextLevelIndex++;
    }

    /**
     * Destroys the current level and generates a level given an index
     * @param levelIndex - The index for which level to generate (0-indexed)
     */
    public void GenerateLevel(int levelIndex)
    {
        DestroyLevel();

        currentLevel = Instantiate(levelTilemapObjects[levelIndex], Vector3.zero, Quaternion.identity);
        currentLevel.transform.SetParent(gridObject.transform, true);
        nextLevelIndex = levelIndex + 1;
    }

    /**
     * Destroys the current level and resets it
     */
    public void RestartLevel()
    {
        DestroyLevel();

        currentLevel = Instantiate(levelTilemapObjects[nextLevelIndex - 1], Vector3.zero, Quaternion.identity);
        currentLevel.transform.SetParent(gridObject.transform, true);
    }

    /**
     * Destroyes the current level if it exists
     */
    public void DestroyLevel()
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
    }
}
