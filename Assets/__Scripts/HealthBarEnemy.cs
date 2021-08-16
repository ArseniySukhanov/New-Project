using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

/// <summary>
/// Class for an enemy's health bar
/// </summary>
public class HealthBarEnemy : MonoBehaviour, IHealthBar
{
    /// <summary>
    /// Maximum health value
    /// </summary>
    private int _maxHealth;
    /// <summary>
    /// Text which shows amount of health
    /// </summary>
    [Header("Set in Inspector")] [SerializeField]
    private Text text;
    [SerializeField] private RectTransform rectTransform;
    
    /// <summary>
    /// Initializes max health
    /// </summary>
    /// <param name="maxHealth">Maximum amount of the health</param>
    public void Initial(int maxHealth)
    {
        _maxHealth = maxHealth;
    }

    /// <summary>
    /// Shows current health
    /// </summary>
    /// <param name="health">Current health amount</param>
    public void CurrentHealth(int health)
    {
        text.text = health + "/" + _maxHealth;
        var value = (float)Math.Ceiling(health * 26f / _maxHealth);
        rectTransform.localScale = new Vector3(value / 26f, 1, 1);
    }

    /// <summary>
    /// Reinitializes health bar
    /// </summary>
    /// <param name="newMaxHealth">New maximum health value</param>
    /// <param name="health">Current health value</param>
    public void ReInitial(int newMaxHealth, int health)
    {
        Initial(newMaxHealth);
        CurrentHealth(health);
    }
}
