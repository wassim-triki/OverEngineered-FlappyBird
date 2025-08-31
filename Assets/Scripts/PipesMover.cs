using System;
using System.Xml.Schema;
using UnityEngine;

public class PipesMover : MonoBehaviour
{
    
    private float _fallbackSpeed = 10f; // used if no controller is set
    
    private bool _frozen;
    private Rigidbody2D _rigidbody;
    
    private DifficultyController _difficulty;
    public void Initialize(DifficultyController d) => _difficulty = d;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_frozen) return;
        float speed = _difficulty? _difficulty.CurrentSpeed : _fallbackSpeed;
        _rigidbody.MovePosition(_rigidbody.position + Vector2.left * speed * Time.fixedDeltaTime);
    }
    public void Freeze()
    {
        _frozen = true;
    }

}
