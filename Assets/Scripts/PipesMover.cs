using System;
using System.Xml.Schema;
using DefaultNamespace;
using UnityEngine;

public class PipesMover : MonoBehaviour, IFreezable
{
    
    private float _fallbackSpeed = 10f; // used if no controller is set
    private Rigidbody2D _rigidbody;
    private DifficultyController _difficulty;
    public bool IsFrozen { get; private set; }
    
    public void Initialize(DifficultyController d) => _difficulty = d;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void OnEnable()
    {
        GameStateManager.OnGameOver += Freeze;
        GameStateManager.OnGamePaused += Freeze;
        GameStateManager.OnGamePlaying += Unfreeze;
    }
    void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnGameOver -= Freeze;
            GameStateManager.OnGamePlaying -= Unfreeze;
        }
    }

    private void FixedUpdate()
    {
        if (IsFrozen) return;
        
        float speed = _difficulty ? _difficulty.CurrentSpeed : _fallbackSpeed;
        _rigidbody.MovePosition(_rigidbody.position + Vector2.left * speed * Time.fixedDeltaTime);
    }
    
    public void Freeze()
    {
        IsFrozen = true;
    }
    
    public void Unfreeze()
    {
        IsFrozen = false;
    }

}
