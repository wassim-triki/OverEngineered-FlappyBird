using System;
using System.Xml.Schema;
using UnityEngine;

public class PillarsMover : MonoBehaviour
{
    [SerializeField] private float speed = 2f; // units/sec to the left
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = Vector2.left * speed;
    }

}
