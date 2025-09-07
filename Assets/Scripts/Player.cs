using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private bool _freezeXWhenIdle = true;

    private bool _isXSnapping;
    private RigidbodyConstraints2D _constraintsBeforeSnap;

    private Vector3 _initPosition;
    private Vector2 _velocityOnPause;

    private bool _hasAutoJumped = false;

    private Rigidbody2D _rigidbody;
    private InputAction _jump;
    private float _holdTimer;
    private bool _movementsEnabled;
    private bool _controlsEnabled;

    // --- NEW: input sampling flags (Update) ---
    private bool _jumpPressedThisFrame;
    private bool _jumpHeld;

    // --- NEW: helper for SmoothDamp X snapping ---
    private float _xSmoothVel; // internal velocity used by SmoothDamp

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _jump = InputSystem.actions.FindAction("Jump");
        _rigidbody.gravityScale = normalGravity;
    }

    void OnEnable() => EnableMovements();
    void OnDisable() => DisableMovements();

    public void ResetPlayer(bool resetAutoJump = true)
    {
        transform.position = _initPosition;
        transform.rotation = Quaternion.identity;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        if (resetAutoJump) ResetAutoJump();
    }

    public void EnableMovements()
    {
        _movementsEnabled = true;
        EnableControls();
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.linearVelocity = _velocityOnPause;
        if (_freezeXWhenIdle)
            _rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX;

    }

    public void DisableMovements()
    {
        _movementsEnabled = false;
        DisableControls();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.linearVelocity = Vector2.zero;
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
        _jumpPressedThisFrame = false;
        _jumpHeld = false;
    }

    // ---- NEW: read inputs in Update to avoid missing frames ----
    void Update()
    {
        if (!_controlsEnabled || _jump == null) return;

        // Block if pointer over UI (same check you used)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            _jumpPressedThisFrame = false;
            _jumpHeld = false;
            return;
        }

        if (_jump.WasPerformedThisFrame())
            _jumpPressedThisFrame = true;

        _jumpHeld = _jump.IsPressed();
    }

    void FixedUpdate()
    {
        if (!_movementsEnabled || _jump == null) return;

        if (_controlsEnabled)
        {
            HandleJump();   // now consumes flags set in Update()
        }

        SnapXToTargetOverTime(-3f, 3f); // renamed + damped snapping
        HandleGravity();
        HandleAutoJump();
        HandleRotation();
    }

    void HandleJump()
    {
        // Start jump (consume one-shot flag)
        if (_jumpPressedThisFrame)
        {
            _jumpPressedThisFrame = false;
            PerformJump();
        }

        // Hold boost (physics time)
        if (_jumpHeld && _holdTimer > 0f && _rigidbody.linearVelocity.y > 0f)
        {
            float strength = _holdTimer / holdTime;
            _rigidbody.linearVelocity += Vector2.up * (holdBoost * strength * Time.fixedDeltaTime);
            _holdTimer -= Time.fixedDeltaTime;
        }
    }

    void HandleGravity()
    {
        if (_rigidbody.linearVelocity.y < -3f)
        {
            _rigidbody.gravityScale = fallGravity; // Falling fast
        }
        else if (_rigidbody.linearVelocity.y > 0f && _jumpHeld && _holdTimer > 0f)
        {
            _rigidbody.gravityScale = holdGravity; // Rising with hold
        }
        else
        {
            _rigidbody.gravityScale = normalGravity; // Normal state
        }
    }

    void HandleRotation()
    {
        // Map velocity to rotation angle (-maxRotation to +maxRotation)
        float targetRotation = Mathf.Clamp(_rigidbody.linearVelocity.y * 5f, -maxRotation, maxRotation);

        // Smoothly rotate towards target (physics time)
        float currentZ = transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f; // unwrap

        float t = rotationSpeed * Time.fixedDeltaTime;
        float newZ = Mathf.LerpAngle(currentZ, targetRotation, t);
        transform.rotation = Quaternion.Euler(0f, 0f, newZ);
    }
    
    void PerformJump()
    {
        AudioManager.I.Play(Sfx.Jump);
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

    public void SnapXToTargetOverTime(float targetX, float smoothTime)
    {
        if (_rigidbody == null || smoothTime <= 0f) return;

        float currentX = transform.position.x;
        float dist = Mathf.Abs(targetX - currentX);

        // If we're not snapping and already at target, ensure X is frozen and bail.
        if (!_isXSnapping && dist < 0.001f)
        {
            if (_freezeXWhenIdle)
                _rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX;
            return;
        }

        // On first tick of a snap: unfreeze X but remember prior constraints.
        if (!_isXSnapping)
        {
            _constraintsBeforeSnap = _rigidbody.constraints;
            if (_freezeXWhenIdle)
                _rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
            _isXSnapping = true;
        }

        // SmoothDamp the *position* along X (critically damped feel)
        float newX = Mathf.SmoothDamp(
            currentX,
            targetX,
            ref _xSmoothVel,
            smoothTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        // Convert to velocity this physics tick
        float requiredVelX = (newX - currentX) / Time.fixedDeltaTime;
        _rigidbody.linearVelocity = new Vector2(requiredVelX, _rigidbody.linearVelocity.y);

        // Close enough? land, stop, and re-freeze X.
        if (Mathf.Abs(targetX - newX) < 0.005f && Mathf.Abs(requiredVelX) < 0.02f)
        {
            _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

            // Re-freeze X (preserve any other constraints the body had)
            _rigidbody.constraints = _freezeXWhenIdle
                ? (_constraintsBeforeSnap | RigidbodyConstraints2D.FreezePositionX)
                : _constraintsBeforeSnap;

            _xSmoothVel = 0f;
            _isXSnapping = false;
        }
    }

}
