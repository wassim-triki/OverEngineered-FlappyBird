using System;
using System.Xml.Schema;
using DefaultNamespace;
using UnityEngine;

public class PipesMover : MonoBehaviour, IFreezable
{
    //TODO: add ability to add manually initial pipe pair statioanary and then moving with difficulty speed when player plays
    private float _fallbackSpeed = 3f; //TODO: try to remove hardcoded for initial manual pipe pair
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
        GameStateManager.OnMenu += Freeze;
    }

    void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnGameOver -= Freeze;
            GameStateManager.OnGamePaused -= Freeze;
            GameStateManager.OnGamePlaying -= Unfreeze;
            GameStateManager.OnMenu -= Freeze;
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
