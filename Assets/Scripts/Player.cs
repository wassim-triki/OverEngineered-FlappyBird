using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 13.5f;
    [SerializeField] private float holdTime = 0.16f;
    [SerializeField] private float holdBoost = 16f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float maxRotation = 45f; // degrees
    [SerializeField] private float rotationSpeed = 10f; // how fast to rotate

    [Header("Gravity")]
    [SerializeField] private float normalGravity = 5.8f;
    [SerializeField] private float holdGravity = 4.8f;
    [SerializeField] private float fallGravity = 10.5f;

    private bool _hasAutoJumped = false;

    private Rigidbody2D _rigidbody;
    private InputAction _jump;
    private float _holdTimer;
    private bool _movementsEnabled;
    private bool _controlsEnabled;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _jump = InputSystem.actions.FindAction("Jump");
        _rigidbody.gravityScale = normalGravity;
    }

    void OnEnable() => EnableMovements();
    void OnDisable() => DisableMovements();

    public void EnableMovements()
    {
        _movementsEnabled = true;
        EnableControls();
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
    public void DisableMovements()
    {
        _movementsEnabled = false;
        DisableControls();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    public void EnableControls()
    {
        _controlsEnabled = true;
        _jump?.Enable();
    }
    public void DisableControls()
    {
        _controlsEnabled = false;
        _jump?.Disable();
    }

    void Update()
    {
        if (!_movementsEnabled || _jump == null) return;

        if (_controlsEnabled)
        {
            HandleJump();
        }
        HandleAutoJump();
        HandleGravity();
        HandleRotation();
    }

    void HandleJump()
    {
        // Start jump
        if (_jump.WasPerformedThisFrame())
        {
            PerformJump();
        }

        // Hold boost
        if (_jump.IsPressed() && _holdTimer > 0f && _rigidbody.linearVelocity.y > 0f)
        {
            float strength = _holdTimer / holdTime;
            _rigidbody.linearVelocity += Vector2.up * (holdBoost * strength * Time.deltaTime);
            _holdTimer -= Time.deltaTime;
        }
    }

    void HandleGravity()
    {
        if (_rigidbody.linearVelocity.y < -3f)
        {
            // Falling fast
            _rigidbody.gravityScale = fallGravity;
        }
        else if (_rigidbody.linearVelocity.y > 0f && _jump.IsPressed() && _holdTimer > 0f)
        {
            // Rising with hold
            _rigidbody.gravityScale = holdGravity;
        }
        else
        {
            // Normal state
            _rigidbody.gravityScale = normalGravity;
        }
    }

    void HandleRotation()
    {
        // Map velocity to rotation angle (-maxRotation to +maxRotation)
        float targetRotation = Mathf.Clamp(_rigidbody.linearVelocity.y * 5f, -maxRotation, maxRotation);
        
        // Smoothly rotate towards target
        float currentZ = transform.eulerAngles.z;
        // Handle angle wrapping (convert 350° to -10°)
        if (currentZ > 180f) currentZ -= 360f;
        
        float newZ = Mathf.LerpAngle(currentZ, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newZ);
    }
    
    void PerformJump()
    {
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpForce);
        _holdTimer = holdTime;
    }
    
    void HandleAutoJump()
    {
        if (_hasAutoJumped) return;
        PerformJump();
        _hasAutoJumped = true;
    }
    
    public void ResetAutoJump() => _hasAutoJumped = false;
}