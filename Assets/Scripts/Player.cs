using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private int jumpForce = 35;
    private InputAction _jumpAction;
    [SerializeField] private int gravityScale = 10;
    

    
    void OnEnable()  => _jumpAction?.Enable();

    void Start()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = gravityScale;
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        if (_jumpAction.WasPerformedThisFrame())
        {
            _rigidbody2D.linearVelocity = Vector2.up * jumpForce;
        }
    }
    
    



    public void EnableControls()  => _jumpAction?.Enable();
    public void DisableControls() => _jumpAction?.Disable();
    

}
