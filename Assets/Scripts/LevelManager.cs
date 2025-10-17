using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Singleton
    public static LevelManager Instance;

    [Header("References")]
    [SerializeField] private GameObject[] levelTilemapObjects;
    [SerializeField] private GameObject gridObject;
    [SerializeField] private GameObject currentLevel;

    private Grid _grid;

    int nextLevelIndex = 1;
    [SerializeField] private Vector3 playerSpawn;


    private void Awake()
    {
        Instance = this;
        _grid = gridObject.GetComponent<Grid>();
    }

    public GameObject getCurrentLevel()
    {
        return currentLevel;
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
        playerSpawn = currentLevel.transform.Find("Player").position;
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
        playerSpawn = currentLevel.transform.Find("Player").position;
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

    /**
     * Gets the player's spawn position for the current level
     * @return Vector3 - The world position of the player's spawn point
     */
    public Vector3 GetPlayerSpawnPosition()
    {         
        return playerSpawn;
    }
}
