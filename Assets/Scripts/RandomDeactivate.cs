using UnityEngine;

public class RandomDeactivate : MonoBehaviour
{
    [SerializeField] float disableChance;
    void Start()
    {
        if (Random.Range(0f, 1f) < disableChance)
        {
            gameObject.SetActive(false);
        }
    }
}
