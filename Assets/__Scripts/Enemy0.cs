using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for an Enemy #0 (Ghost)
/// </summary>
public class Enemy0 : Enemy
{
    // Parameter for an animator
    private static readonly int State = Animator.StringToHash("State");
    
    /// <summary>
    /// Initialization of the enemy
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        Animator.SetInteger(State,0);
    }
    
    /// <summary>
    /// Starts AI
    /// </summary>
    protected void Start()
    {
        StartCoroutine(nameof(MoveAttack));
    }
    
    
    /// <summary>
    /// Coroutine for AI of the ghosts
    /// </summary>
    /// <remarks>Current AI: achieve aim and attack it</remarks>
    private IEnumerator MoveAttack()
    {
        var enemyStart = Tile.TransToTile(transform.position);
        yield return new WaitUntil(()=>(enemyStart-Tile.TransToTile(Hero.transform.position)).magnitude<7f);
        Pathfinding.AStarSearchAdaptive((int)enemyStart.x, (int)enemyStart.y,new Pathfinding.ActionAim(Hero.gameObject));
        while (Pathfinding.Path.Count != 0)
        {
            Animator.SetInteger(State,1);
            var tCell = Pathfinding.Path.Pop();
            var aimTrans = (Vector3)Tile.TileToTrans(new Vector2(tCell.X, tCell.Y)) + 0.5f * Vector3.up;
            var tSpeed = speed *
                         (aimTrans - transform.position).normalized;
            if (tSpeed.x > 0)
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            if (tSpeed.x < 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position += tSpeed * Time.deltaTime;
            while (Vector3.Dot(aimTrans - transform.position, tSpeed) > 0)
            {
                yield return null;
                transform.position += tSpeed * Time.deltaTime;
            }

            transform.position = aimTrans;
            enemyStart = Tile.TransToTile(transform.position);
            Animator.SetInteger(State,2);
            yield return new WaitUntil(()=>(enemyStart-Tile.TransToTile(Hero.transform.position)).magnitude<7f);
            Pathfinding.AStarSearchAdaptive((int)enemyStart.x, (int)enemyStart.y,new Pathfinding.ActionAim(Hero.gameObject));
        }
        Animator.SetInteger(State,1);
        if ((transform.position-Hero.transform.position).x < 0)
                transform.rotation = Quaternion.Euler(0, 180f, 0);
        yield return new WaitForSeconds(timeBeforeAttack);
        if(2>Math.Max(Math.Abs((Tile.TransToTile(transform.position)-Tile.TransToTile(Hero.transform.position)).x),Math.Abs((Tile.TransToTile(transform.position)-Tile.TransToTile(Hero.transform.position)).y)))
            TryAttack();
        StartCoroutine(nameof(MoveAttack));
    }
    
    /// <summary>
    /// Attacks player
    /// </summary>
    private void TryAttack()
    {
        Hero.GetComponent<IDamageable>().ReceiveDamage(1);
    }
}
