using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Interface for processing damage receive
/// </summary>
interface IDamageable
{
    void ReceiveDamage(int damage);
}