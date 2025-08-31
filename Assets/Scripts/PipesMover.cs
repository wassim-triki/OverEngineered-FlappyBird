using System;
using System.Xml.Schema;
using UnityEngine;

public class PipesMover : MonoBehaviour
{
    [SerializeField] private float speed = 2f; 
    private bool _frozen;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_frozen) return;
        _rigidbody.MovePosition(_rigidbody.position + Vector2.left * speed * Time.fixedDeltaTime);
    }
    public void Freeze()
    {
        _frozen = true;
    }

}
