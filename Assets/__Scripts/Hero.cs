using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Class for main character
/// </summary>
public class Hero : MonoBehaviour, IDamageable
{
    private Controls _controls;
    private Camera _mainCamera;
    private Pathfinding _pathfinding;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    /// <summary>
    /// Maximum amount of hp
    /// </summary>
    [Header("Set in Inspector")] [SerializeField]
    private int maxHealth;

    /// <summary>
    /// Health bar for hero
    /// </summary>
    [SerializeField] private GameObject healthBar;
    private IHealthBar _healthBar;
    /// <summary>
    /// Class for a pause menu
    /// </summary>
    [SerializeField] private PauseMenu pauseMenu;
    /// <summary>
    /// Minimal time between attacks
    /// </summary>
    [SerializeField] private float minTimeBetweenAttacks = 0.2f;
    /// <summary>
    /// Time of invincibility after received damage
    /// </summary>
    [SerializeField] private float invincibilityTimeSpawn = 0.5f;
    /// <summary>
    /// Speed of the hero
    /// </summary>
    [SerializeField] private float speed = 5.0f;
    /// <summary>
    /// Modern aim to move to or attack which
    /// </summary>
    private Pathfinding.ActionAim _modernActionAim;
    /// <summary>
    /// Queue of saved actions to do after modern one
    /// </summary>
    /// <remarks>Is not made yet</remarks>
    private Queue<Pathfinding.ActionAim> _futureActions;
    // Parameters for animation
    private static readonly int State1 = Animator.StringToHash("State");
    private static readonly int For = Animator.StringToHash("For");
    
    /// <summary>
    /// Modern number of hp
    /// </summary>
    [Header("Dynamical data")] [SerializeField]
    private int health;
    /// <summary>
    /// State in which hero is now
    /// </summary>
    [SerializeField] private State state;
    /// <summary>
    /// Time before invincibility after receiving damage would end
    /// </summary>
    [SerializeField] private float invincible;
    /// <summary>
    /// Time before next attack could be 
    /// </summary>
    [SerializeField] private float currentPauseBeforeAttack;

    private enum State
    {
        Idle,
        Motion,
        Fighting
    }
    
    /// <summary>
    /// Initialization of the character on the start of the scene
    /// </summary>
    private void Awake()
    {
        invincible = 0;
        health = maxHealth;
        _healthBar = healthBar.GetComponent<IHealthBar>();
        _healthBar.Initial(maxHealth);
        currentPauseBeforeAttack = 0;
        _futureActions = new Queue<Pathfinding.ActionAim>();
        state = State.Idle;
        _pathfinding = GetComponent<Pathfinding>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
    
    /// <summary>
    /// Player movement and attack starter
    /// </summary>
    /// <param name="callbackContext">Information from the input</param>
    public void MoveAttackStart(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.started || PauseMenu.IsPaused) return;
        var mousePosition = _controls.Player.MousePlace.ReadValue<Vector2>();
        var playerStart = Tile.TransToTile(transform.position);
        // Checks that the action was not invoked by pressing UI buttons
        var ray = _mainCamera.ScreenPointToRay(mousePosition);
        Physics.Raycast(ray, out _); 
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        for (var i = 0; results.Count > i; i++)
        {
            if (results[i].gameObject.layer == 5) return;
        }
        //---------------------------------------------------------------
        // Check if player have chosen an object, not a location
        _mainCamera.GetComponent<Physics2DRaycaster>().Raycast(eventDataCurrentPosition,results);
        if (results.Count > 0)
        {
            // Attack initialization
            
            if (state == State.Motion)
            {
                StopCoroutine(nameof(Move));
                _futureActions.Clear();
            }
            if (state == State.Fighting)
            { 
                if (_modernActionAim.Target != null)
                    _modernActionAim.Target.GetComponent<Enemy>().UnChosen();
                StopCoroutine(nameof(Attack));
                _futureActions.Clear();
            }
            results[0].gameObject.GetComponent<Enemy>().Chosen();
            state = State.Fighting;
            _animator.SetInteger(State1,1);
            _modernActionAim = new Pathfinding.ActionAim(results[0].gameObject);
            StartCoroutine(nameof(Attack));
        }
        else
        {
            // Check if location which is chosen is not achievable
            var mousePlace = Tile.TransToTile(_mainCamera.ScreenToWorldPoint(mousePosition));
            if (Map.TileMap[(int)playerStart.x, (int)playerStart.y] < 0 ||
                Map.TileMap[(int)playerStart.x, (int)playerStart.y] > 223) return;
            if (mousePlace.x > 255 || mousePlace.y > 255 || mousePlace.x < 0 || mousePlace.y < 0 ||
                Map.TileMap[(int)mousePlace.x, (int)mousePlace.y] < 0 ||
                Map.TileMap[(int)mousePlace.x, (int)mousePlace.y] > 223) return;
            
            // Movement initialization
            
            if (state == State.Motion)
            {
                StopCoroutine(nameof(Move));
                _futureActions.Clear();
            }
            if (state == State.Fighting)
            { 
                if (_modernActionAim.Target != null)
                    _modernActionAim.Target.GetComponent<Enemy>().UnChosen();
                StopCoroutine(nameof(Attack));
                _futureActions.Clear();
            }

            state = State.Motion;
            _animator.SetInteger(State1, 1);
            _modernActionAim = new Pathfinding.ActionAim((int)mousePlace.x, (int)mousePlace.y);
            _pathfinding.AStarSearchAdaptive((int)playerStart.x, (int)playerStart.y,_modernActionAim);
            StartCoroutine(nameof(Move));
        }
    }
    
    /// <summary>
    /// Coroutine for movement
    /// </summary>
    private IEnumerator Move()
    {
        while (_pathfinding.Path.Count != 0)
        {
            var tCell = _pathfinding.Path.Pop();
            var aimTrans = (Vector3)Tile.TileToTrans(new Vector2(tCell.X, tCell.Y)) + 0.5f * Vector3.up;
            var tSpeed = speed *
                         (aimTrans - transform.position).normalized;
            _animator.SetBool(For, !(tSpeed.y > 0));
            if (tSpeed.x > 0)
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            if (tSpeed.x < 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position += tSpeed * Time.deltaTime;
            while (Vector3.Dot(aimTrans - transform.position, tSpeed) > 0)
            {
                _mainCamera.transform.position = transform.position + Vector3.back * 10;
                yield return null;
                transform.position += tSpeed * Time.deltaTime;
            }

            transform.position = aimTrans;
            _mainCamera.transform.position = transform.position + Vector3.back * 10;
            yield return null;
        }
        state = State.Idle;
        _animator.SetInteger(State1, 0);
    }
    
    /// <summary>
    /// Coroutine for attack
    /// </summary>
    private IEnumerator Attack()
    {   
        var playerStart = Tile.TransToTile(transform.position);
        _pathfinding.AStarSearchAdaptive((int)playerStart.x, (int)playerStart.y,_modernActionAim);
        while (_pathfinding.Path.Count != 0)
        {
            var tCell = _pathfinding.Path.Pop();
            var aimTrans = (Vector3)Tile.TileToTrans(new Vector2(tCell.X, tCell.Y)) + 0.5f * Vector3.up;
            var tSpeed = speed *
                         (aimTrans - transform.position).normalized;
            _animator.SetBool(For, !(tSpeed.y > 0));
            if (tSpeed.x > 0)
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            if (tSpeed.x < 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position += tSpeed * Time.deltaTime;
            while (Vector3.Dot(aimTrans - transform.position, tSpeed) > 0)
            {
                _mainCamera.transform.position = transform.position + Vector3.back * 10;
                yield return null;
                transform.position += tSpeed * Time.deltaTime;
            }

            transform.position = aimTrans;
            _mainCamera.transform.position = transform.position + Vector3.back * 10;
            yield return null;
            _pathfinding.AStarSearchAdaptive((int)Tile.TransToTile(transform.position).x,(int)Tile.TransToTile(transform.position).y,new Pathfinding.ActionAim(_modernActionAim.Target));
        }
        TryAttack();
        state = State.Idle;
        _modernActionAim.Target.GetComponent<Enemy>().UnChosen();
    }
    
    /// <summary>
    /// Attack function
    /// </summary>
    /// <remarks>It is played when as a part of the attack action player achieved prey</remarks>
    private void TryAttack()
    {
        if ((transform.position-_modernActionAim.Target.transform.position).x < 0)
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        _animator.SetBool(For, !((transform.position - _modernActionAim.Target.transform.position).y < 0));
        if (currentPauseBeforeAttack > 0) return;
        _animator.SetInteger(State1, 2); 
        _modernActionAim.Target.GetComponent<IDamageable>().ReceiveDamage(2);
        currentPauseBeforeAttack = minTimeBetweenAttacks;
        StartCoroutine(PauseBetweenAttack());
    }
    
    /// <summary>
    /// Method for processing received damage
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    public void ReceiveDamage(int damage)
    {
        if (invincible > 0) return;
        health -= damage;
        _healthBar.CurrentHealth(health);
        _spriteRenderer.color = Color.red;
        if (health < 1)
        {
            pauseMenu.Invoke(nameof(PauseMenu.ReturnToMenu),invincibilityTimeSpawn);
        }
        invincible = invincibilityTimeSpawn;
        StartCoroutine(Invincibility());
    }

    /// <summary>
    /// Coroutine which is used as a timer for pause between attacks
    /// </summary>
    private IEnumerator PauseBetweenAttack()
    {
        while (currentPauseBeforeAttack > 0)
        {
            yield return null;
            currentPauseBeforeAttack -= Time.deltaTime;
        }
        _animator.SetInteger(State1,0);
    }
    
    /// <summary>
    /// Coroutine which is used as a timer for invincibility between damage
    /// </summary>
    private IEnumerator Invincibility()
    {
        while (invincible > 0)
        {
            invincible -= Time.deltaTime;
            yield return null;
        }
        _spriteRenderer.color = Color.white;
    }
}
