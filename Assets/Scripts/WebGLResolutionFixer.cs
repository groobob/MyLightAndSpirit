using UnityEngine;

/// <summary>
/// Fixes WebGL canvas resolution issues by forcing proper canvas sizing on scene load
/// This solves the zoom issue where the game appears zoomed in until browser zoom is changed
/// Add this component to any GameObject in scenes that don't have SceneLoader
/// </summary>
public class WebGLResolutionFixer : MonoBehaviour
{
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(ForceResize());
#endif
    }

    private System.Collections.IEnumerator ForceResize()
    {
        yield return new WaitForSeconds(0.5f);
        Screen.SetResolution(Screen.width, Screen.height, false);
    }
}


