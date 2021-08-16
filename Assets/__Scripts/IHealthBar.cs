using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Interface for process showcase of the health 
/// </summary>
interface IHealthBar
{
    void Initial(int maxHealth);
    void CurrentHealth(int health);
    void ReInitial(int newMaxHealth, int health);
}
