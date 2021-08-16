using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    protected Pathfinding Pathfinding;
    private SpriteRenderer _spriteRenderer;
    private Material _material;
    protected Animator Animator;
    
    /// <summary>
    /// Maximum amount of hp
    /// </summary>
    [Header("Set in Inspector:Enemy")] [SerializeField]
    protected int maxHealth = 1;
    
    /// <summary>
    /// Health bar for enemy
    /// </summary>
    private static GameObject _enemyHealthBar;
    private IHealthBar _healthBar;
    
    /// <summary>
    /// Time before first and every next attack
    /// </summary>
    [SerializeField] protected float timeBeforeAttack = 1f;
    /// <summary>
    /// Speed of the enemy
    /// </summary>
    [SerializeField] protected float speed = 5.0f;
    /// <summary>
    /// Time while the damage shown on the enemy
    /// </summary>
    [SerializeField] protected float timeOfHurtShow =0.2f;
    /// <summary>
    /// Class for a pause menu
    /// </summary>
    private static PauseMenu _pauseMenu;
    /// <summary>
    /// Main character GameObject
    /// </summary>
    protected GameObject Hero;
    /// <summary>
    /// All enemies on the map
    /// </summary>
    /// <remarks>Could be used for checking the ending of the game and multithreading AI of enemies</remarks>
    private static List<Enemy> _enemies; 

    /// <summary>
    /// Modern number of the hp
    /// </summary>
    [Header("Dynamical data")]
    [SerializeField] protected int health;
    /// <summary>
    /// Time before ending showcase of the damage
    /// </summary>
    [SerializeField] protected float timeOfHurt;

    //Parameter for an outlining on enemy
    private static readonly int OutlineColor = Shader.PropertyToID("Outline_Color");
    
    /// <summary>
    /// Initialization of the enemy
    /// </summary>
    protected virtual void Awake()
    {
        _pauseMenu = GameObject.FindWithTag("Pause").GetComponent<PauseMenu>();
        _enemies ??= new List<Enemy>();
        _enemies.Add(this);
        health = maxHealth;
        timeOfHurt = 0;
        _enemyHealthBar ??= GameObject.FindWithTag("UIHealthBarEnemy");
        _enemyHealthBar.SetActive(false);
        _healthBar = _enemyHealthBar.GetComponent<IHealthBar>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
        Animator = GetComponent<Animator>();
        Hero = GameObject.FindWithTag("Player");
        Pathfinding = GetComponent<Pathfinding>();
    }
    
    /// <summary>
    /// Method which is used by player to show enemy that he received damage
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    public void ReceiveDamage(int damage)
    {
        health -= damage;
        _healthBar.CurrentHealth(health);
        if (health < 1)
        {
            _enemyHealthBar.SetActive(false);
            if (_enemies.Count == 1)
                _pauseMenu.Invoke(nameof(PauseMenu.ReturnToMenu),1f); 
            Destroy(gameObject);
        }
        _spriteRenderer.color = Color.red;
        if (timeOfHurt > 0) 
            StopCoroutine(nameof(Hurt));
        timeOfHurt = timeOfHurtShow;
        StartCoroutine(nameof(Hurt));
    }
    
    /// <summary>
    /// Coroutine function for ending showing that enemy is damaged after timeOfHurtShow seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator Hurt()
    {
        while (timeOfHurt > 0)
        {
            yield return null;
            timeOfHurt -= Time.deltaTime;
        }
        _spriteRenderer.color = Color.white;
    }

    /// <summary>
    /// Method which is used by the player to show that the aim is chosen and show its stats
    /// </summary>
    public void Chosen()
    {
        _enemyHealthBar.SetActive(true);
        _material.SetColor(OutlineColor, Color.green);
        _healthBar.ReInitial(maxHealth,health);
    }
    
    /// <summary>
    /// Method which is used by the player to stop showing that the one is choosing the enemy
    /// </summary>
    public void UnChosen()
    {
        _material.SetColor(OutlineColor, Color.clear);
    }
    /// <summary>
    /// Used for reinitialization of the health bar for enemies on the start
    /// </summary>
    public static void OnMapCreation()
    {
        _enemyHealthBar = null;
    }
    
}
