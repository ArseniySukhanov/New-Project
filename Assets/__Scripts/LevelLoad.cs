using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class for loading screens
/// </summary>
public class LevelLoad : MonoBehaviour
{
    /// <summary>
    /// GameObject which started loading process
    /// </summary>
    [SerializeField] private GameObject starter;
    /// <summary>
    /// Loading bar
    /// </summary>
    [SerializeField] private Slider loadingBar;
    /// <summary>
    /// Scene to load
    /// </summary>
    [SerializeField] private int loadingScene;
    
    /// <summary>
    /// Start of the loading process
    /// </summary>
    private void OnEnable()
    {
        if (starter!=null)
            starter.SetActive(false);
        StartCoroutine(LoadScene());
    }
    
    /// <summary>
    /// Coroutine for a loading screen
    /// </summary>
    private IEnumerator LoadScene()
    {
        var operation = SceneManager.LoadSceneAsync(loadingScene);
        while ( !operation.isDone )
        {
            var progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBar.value = progress;
            yield return null;
        }
    }
    
}
