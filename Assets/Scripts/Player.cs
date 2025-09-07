using System.Collections;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening; // added

public class Player : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 13.5f;
    [SerializeField] private float holdTime = 0.16f;
    [SerializeField] private float holdBoost = 16f;
    
    // === Soft Top Clamp (feel-first) ===
    [Header("Top Clamp")]
    [SerializeField, Range(0f, 0.25f)]
    private float viewportTopMargin = 0.08f;  // how far below the top edge we place the limit (in normalized viewport units)

    [SerializeField, Min(0f)]
    private float softZoneWorld = 0.9f;       // height of the "soft" band (world units) beneath the limit

    [SerializeField, Min(0f)]
    private float clampPushStrength = 18f;    // downward push when inside soft band (units/sec^2)

    [SerializeField, Min(0f)]
    private float clampDamping = 10f;         // scales how quickly upward velocity is reduced

    private Camera _cam;
    
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

    private bool _hasAutoJumped;

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
    private Coroutine _xSnapRoutine; // coroutine handle for X snapping
    private float smoothTime = 2f;
    private float targetX = -3;

    private Tween _menuScaleTween; // menu appearance tween
    private Vector3 _originalScale;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _jump = InputSystem.actions.FindAction("Jump");
        _rigidbody.gravityScale = normalGravity;
        _initPosition = transform.position; // capture initial spawn position
        _originalScale = transform.localScale; // store original scale
    }

    void OnEnable() => EnableMovements();
    void OnDisable() => DisableMovements();

    void OnDestroy()
    {
        if (_menuScaleTween != null && _menuScaleTween.IsActive()) _menuScaleTween.Kill();
    }

    public void ResetPlayer(bool resetAutoJump = true)
    {
        CancelSnapX(); // ensure no leftover snapping continues into menu
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
        CancelSnapX(); // stop snapping while disabled
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
            HandleJump();
        }

        HandleGravity();
        HandleTopSoftClamp();
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
    
    public void StartSnapX()
    {
        if (_xSnapRoutine != null)
        {
            StopCoroutine(_xSnapRoutine);
            _xSnapRoutine = null;
        }
        _xSnapRoutine = StartCoroutine(SnapXRoutine());
    }

    public void CancelSnapX()
    {
        if (_xSnapRoutine != null)
        {
            StopCoroutine(_xSnapRoutine);
            _xSnapRoutine = null;
        }
        if (_isXSnapping)
        {
            _rigidbody.constraints = _freezeXWhenIdle
                ? (_constraintsBeforeSnap | RigidbodyConstraints2D.FreezePositionX)
                : _constraintsBeforeSnap;
            _isXSnapping = false;
            _xSmoothVel = 0f;
        }
    }

    private IEnumerator SnapXRoutine()
    {
        if (_rigidbody == null || smoothTime <= 0f) yield break;

        float currentX = transform.position.x;
        float dist = Mathf.Abs(targetX - currentX);
        if (!_isXSnapping && dist < 0.001f)
        {
            if (_freezeXWhenIdle)
                _rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX;
            yield break;
        }

        // Setup for snapping (unfreeze X)
        _constraintsBeforeSnap = _rigidbody.constraints;
        if (_freezeXWhenIdle)
            _rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        _isXSnapping = true;
        _xSmoothVel = 0f;

        while (true)
        {
            if (!_movementsEnabled)
            {
                // abort if movements disabled (e.g., menu / game over)
                break;
            }
            currentX = transform.position.x;
            float newX = Mathf.SmoothDamp(
                currentX,
                targetX,
                ref _xSmoothVel,
                smoothTime,
                Mathf.Infinity,
                Time.fixedDeltaTime);

            float requiredVelX = (newX - currentX) / Time.fixedDeltaTime;
            _rigidbody.linearVelocity = new Vector2(requiredVelX, _rigidbody.linearVelocity.y);

            bool done = Mathf.Abs(targetX - newX) < 0.005f && Mathf.Abs(requiredVelX) < 0.02f;
            if (done)
            {
                _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
                transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        // cleanup (both normal completion & aborted)
        _rigidbody.constraints = _freezeXWhenIdle
            ? (_constraintsBeforeSnap | RigidbodyConstraints2D.FreezePositionX)
            : _constraintsBeforeSnap;
        _xSmoothVel = 0f;
        _isXSnapping = false;
        _xSnapRoutine = null;
    }
    void HandleTopSoftClamp()
    {
        if (!_cam) _cam = Camera.main;
        if (!_cam) return;

        // Compute the world Y of the top limit (with viewport margin)
        // We use player's Z so the conversion works for ortho/persp.
        float depth = Mathf.Abs(_cam.transform.position.z - transform.position.z);
        Vector3 topWorld = _cam.ViewportToWorldPoint(new Vector3(0.5f, 1f - viewportTopMargin, depth));
        float maxY = topWorld.y;

        // Soft band starts this far below the top limit
        float softStartY = maxY - softZoneWorld;

        Vector3 pos = transform.position;
        Vector2 vel = _rigidbody.linearVelocity;

        if (pos.y >= softStartY)
        {
            // How deep into the soft band are we? (0 at softStart, 1 at max)
            float t = Mathf.InverseLerp(softStartY, maxY, pos.y);

            // 1) Reduce upward velocity as we approach the cap
            if (vel.y > 0f)
            {
                // Scale down upward speed proportionally to depth in band
                float damping = clampDamping * t * Time.fixedDeltaTime;
                vel.y = Mathf.Lerp(vel.y, 0f, damping);
            }

            // 2) Apply a gentle downward acceleration that ramps up in the band
            float push = clampPushStrength * t * Time.fixedDeltaTime;
            vel.y -= push;

            // 3) Safety net: never allow crossing above the absolute cap
            if (pos.y > maxY)
            {
                pos.y = maxY;
            }

            _rigidbody.linearVelocity = vel;
            transform.position = pos;
        }
    }

    public void AnimateMenuAppear()
    {
        // Immediately set scale to 0, then after delay animate to original scale
        if (_menuScaleTween != null && _menuScaleTween.IsActive()) _menuScaleTween.Kill();
        transform.localScale = Vector3.zero;
        _menuScaleTween = transform.DOScale(_originalScale, 0.5f)
            .SetDelay(0.25f)
            .SetEase(Ease.OutBack);
    }

    public void EnsureNormalScale()
    {
        if (_menuScaleTween != null && _menuScaleTween.IsActive())
        {
            _menuScaleTween.Kill();
        }
        transform.localScale = _originalScale;
    }
}
