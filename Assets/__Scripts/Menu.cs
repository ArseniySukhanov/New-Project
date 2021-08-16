using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for a main menu
/// </summary>
public class Menu : MonoBehaviour
{
    /// <summary>
    /// Loading screen
    /// </summary>
    [SerializeField] private GameObject loadingScreen;
    
    /// <summary>
    /// Method which starts new game
    /// </summary>
    public void NewGame()
    {
        Time.timeScale = 1f;
        PauseMenu.IsPaused = false;
        loadingScreen.SetActive(true);
    }
    
    /// <summary>
    /// Method which exits from the game
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
