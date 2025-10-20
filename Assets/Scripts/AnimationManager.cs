using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    [SerializeField] Animator transitionAnimator;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayTransition()
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Play");
        }
    }
}
