using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Refs")]
    private Rigidbody2D _rb;
    private InputAction _jump;

    [Header("Jump")]
    [Range(5f, 200f)] 
    [SerializeField] private float initialJumpVel = 45f;    // massive jump for huge sprite
    [Range(0.01f, 0.2f)] 
    [SerializeField] private float maxHoldTime = 0.05f;     // tiny hold time for snappy feel
    [Range(50f, 200f)] 
    [SerializeField] private float holdBoostVelPerSec = 80f; // strong brief boost

    [Header("Gravity Tuning")]
    [Range(0f, 200f)] 
    [SerializeField] private float baseGravityScale = 15f;   // heavy baseline for big object
    [Range(0f, 200f)] 
    [SerializeField] private float fallGravityScale = 25f;   // fast drops
    [Range(0f, 200f)] 
    [SerializeField] private float cutGravityScale = 35f;    // immediate plummet
    [Range(0f, 200f)] 
    [SerializeField] private float holdGravityScale = 8f;    // still heavy while holding

    private float _holdTimer;         // counts down from maxHoldTime
    private bool _wasHeldLastFrame;   // for detecting release
    private bool _controlsEnabled;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _jump = InputSystem.actions.FindAction("Jump");
        _rb.gravityScale = baseGravityScale;
    }

    void OnEnable()
    {
        _controlsEnabled = true;
        _jump?.Enable();
    }

    void OnDisable()
    {
        _controlsEnabled = false;
        _jump?.Disable();
    }

    public void EnableControls()  { _controlsEnabled = true;  _jump?.Enable(); }
    public void DisableControls() { _controlsEnabled = false; _jump?.Disable(); }

    void Update()
    {
        if (!_controlsEnabled || _jump == null) return;

        // 1) Start of jump: set a clean upward velocity and prime hold timer
        if (_jump.WasPerformedThisFrame())
        {
            var v = _rb.linearVelocity;
            v.y = initialJumpVel;
            _rb.linearVelocity = v;
            _holdTimer = maxHoldTime;
        }

        bool held = _jump.IsPressed();
        bool rising = _rb.linearVelocity.y > 3f;    // much higher threshold for massive sprite
        bool falling = _rb.linearVelocity.y < -3f;

        // 2) While held (and we still have hold time), add upward acceleration
        if (held && _holdTimer > 0f && rising)
        {
            // Apply boost more smoothly - scale down as hold time expires
            float holdStrength = _holdTimer / maxHoldTime;
            _rb.linearVelocity += Vector2.up * (holdBoostVelPerSec * holdStrength * Time.deltaTime);
            _holdTimer -= Time.deltaTime;
        }

        // 3) Gravity shaping for feel
        if (falling)
        {
            _rb.gravityScale = fallGravityScale;      
        }
        else if (rising)
        {
            // Check for early release (cut jump)
            if (!held && _wasHeldLastFrame && _holdTimer > 0f)
            {
                _rb.gravityScale = cutGravityScale;   // immediate heavy gravity on release
                _holdTimer = 0f; // prevent further hold boost
            }
            else if (held && _holdTimer > 0f)
            {
                _rb.gravityScale = holdGravityScale;  // light gravity while boosting
            }
            else
            {
                _rb.gravityScale = baseGravityScale;  // normal gravity
            }
        }
        else
        {
            _rb.gravityScale = baseGravityScale;      // at apex or stationary
        }

        _wasHeldLastFrame = held;
    }
}