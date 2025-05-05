using UnityEngine;
using PlayerActions;

[RequireComponent(typeof(Rigidbody2D))]
public class SamuraiController : MonoBehaviour
{
    public event System.Action OnJumpStarted;
    public event System.Action OnLanded;

    private Rigidbody2D _rb;
    private PlayerInputActions _input;
    private Vector2 _moveInput;
    private bool _isRunning;

    private bool _isGrounded;
    private bool _jumpRequested;
    private bool _wasGroundedLastFrame;
    private bool _justLanded;

    [Header("References")]
    [SerializeField] private Transform visuals;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;          
    [SerializeField] private float groundCheckRadius = 0.1f;  
    [SerializeField] private LayerMask groundLayer;

    public bool IsAttacking { get; set; }
    public bool IsFalling => _rb.linearVelocity.y < -0.1f && !_isGrounded;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = new PlayerInputActions();
    }

    void OnEnable()
    {
        _input.Enable();

        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _input.Player.Run.performed += ctx => _isRunning = true;
        _input.Player.Run.canceled += ctx => _isRunning = false;

        _input.Player.Jump.performed += ctx => TryJump();

    }

    void OnDisable()
    {
        _input.Disable();
    }

    void Update()
    {
        HandleVisualFlip();
    }

    void FixedUpdate()
    {
        // Ground check first
        CheckGrounded();

        // Apply movement only if not attacking
        float speed = _isRunning ? runSpeed : walkSpeed;
        float horizontal = IsAttacking ? 0f : _moveInput.x * speed;

        // Stop movement if grounded and no input
        if (_justLanded)
        {
            horizontal = 0f;
        }

        // Apply horizontal + preserve vertical
        _rb.linearVelocity = new Vector2(horizontal, _rb.linearVelocity.y);


        // Apply jump (even during attack if needed)
        if (_jumpRequested)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _jumpRequested = false;
        }
    }

    private void HandleVisualFlip()
    {
        if (_moveInput.x != 0 && visuals != null)
        {
            Vector3 scale = visuals.localScale;
            scale.x = _moveInput.x > 0 ? 1 : -1;
            visuals.localScale = scale;
        }
    }

    private void TryJump()
    {
        if (_isGrounded && !IsAttacking)
        {
            _jumpRequested = true;
            OnJumpStarted?.Invoke(); 
        }
    }

    private void CheckGrounded()
    {
        _wasGroundedLastFrame = _isGrounded;
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        _justLanded = _isGrounded && !_wasGroundedLastFrame;

        if (_justLanded)
        {
            OnLanded?.Invoke(); // used in AnimatorController to trigger "Land"
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // Public accessors for animation logic
    public float SpeedParameter
    {
        get
        {
            if (_moveInput.x == 0) return 0f;
            return _isRunning ? 1f : 0.5f;
        }
    }

    public bool IsMoving => _moveInput.x != 0;
    public bool IsRunning => _isRunning;
}
