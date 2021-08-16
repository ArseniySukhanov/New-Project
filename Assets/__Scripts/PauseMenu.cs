using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// Shows if the game is paused
    /// </summary>
    public static bool IsPaused;
    /// <summary>
    /// Pause menu
    /// </summary>
    [SerializeField] private GameObject uiPauseMenu;
    /// <summary>
    /// Button for a pause activation
    /// </summary>
    [SerializeField] private GameObject button;
    /// <summary>
    /// Loading screen
    /// </summary>
    [SerializeField] private GameObject loadingScreen;
    
    /// <summary>
    /// Method for the game pause
    /// </summary>
    public void Pause()
    {
        IsPaused = true;
        uiPauseMenu.SetActive(true);
        button.SetActive(false);
        Time.timeScale = 0f;
    }
    
    /// <summary>
    /// Method for the game continue
    /// </summary>
    public void Resume()
    {
        IsPaused = false;
        uiPauseMenu.SetActive(false);
        button.SetActive(true);
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// Method for the game return to a menu screen
    /// </summary>
    public void ReturnToMenu()
    {
        loadingScreen.SetActive(true);
    }
}
