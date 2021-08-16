using System;
using UnityEngine;

/// <summary>
/// Class for a hero's health bar
/// </summary>
public class HealthBar : MonoBehaviour, IHealthBar
{
    [SerializeField] private GameObject mask;
    /// <summary>
    /// Maximum health value
    /// </summary>
    private int _maxHealth;
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
        var pos = mask.transform.localPosition;
        pos.y = 6.25f * (float)Math.Ceiling(health * 16f / _maxHealth);
        mask.transform.localPosition = pos;
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