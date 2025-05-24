using UnityEngine;
using PlayerActions;

[RequireComponent(typeof(Rigidbody2D))]
public class SamuraiController : MonoBehaviour
{
    public event System.Action OnJumpStarted;
    public event System.Action OnLanded;

    private Rigidbody2D m_rigidBody;
    private PlayerInputActions m_input;
    private Vector2 m_moveInput;
    private bool m_isRunning;
    public bool IsRunning => m_isRunning;

    private bool m_isGrounded;
    private bool m_jumpRequested;
    private bool m_wasGroundedLastFrame;
    private bool m_justLanded;

    [Header("References")]
    [SerializeField] private Transform m_visuals;

    [Header("Movement Settings")]
    [SerializeField] private float m_walkSpeed = 2f;
    [SerializeField] private float m_runSpeed = 4f;

    [Header("Jump Settings")]
    [SerializeField] private float m_jumpForce = 12f;
    [SerializeField] private Transform m_groundCheck;          
    [SerializeField] private float m_groundCheckRadius = 0.1f;  
    [SerializeField] private LayerMask m_groundLayer;

    public bool IsAttacking { get; set; }
    public bool IsFalling => m_rigidBody.linearVelocity.y < -0.1f && !m_isGrounded;
    public bool IsMoving => m_moveInput.x != 0;
    


    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_input = new PlayerInputActions();
    }

    void OnEnable()
    {
        m_input.Enable();

        m_input.Player.Move.performed += ctx => m_moveInput = ctx.ReadValue<Vector2>();
        m_input.Player.Move.canceled += ctx => m_moveInput = Vector2.zero;

        m_input.Player.Run.performed += ctx => m_isRunning = true;
        m_input.Player.Run.canceled += ctx => m_isRunning = false;

        m_input.Player.Jump.performed += ctx => TryJump();
    }

    void OnDisable()
    {
        m_input.Disable();
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
        float speed = m_isRunning ? m_runSpeed : m_walkSpeed;
        float horizontal = IsAttacking ? 0f : m_moveInput.x * speed;

        // Stop movement if grounded and no input
        if (m_justLanded)
        {
            horizontal = 0f;
        }

        // Apply horizontal + preserve vertical
        m_rigidBody.linearVelocity = new Vector2(horizontal, m_rigidBody.linearVelocity.y);


        // Apply jump (even during attack if needed)
        if (m_jumpRequested)
        {
            m_rigidBody.linearVelocity = new Vector2(m_rigidBody.linearVelocity.x, m_jumpForce);
            m_jumpRequested = false;
        }
    }

    private void HandleVisualFlip()
    {
        if (m_moveInput.x != 0 && m_visuals != null)
        {
            Vector3 scale = m_visuals.localScale;
            scale.x = m_moveInput.x > 0 ? 1 : -1;
            m_visuals.localScale = scale;
        }
    }

    private void TryJump()
    {
        if (m_isGrounded && !IsAttacking)
        {
            m_jumpRequested = true;
            OnJumpStarted?.Invoke(); 
        }
    }

    private void CheckGrounded()
    {
        m_wasGroundedLastFrame = m_isGrounded;
        m_isGrounded = Physics2D.OverlapCircle(m_groundCheck.position, m_groundCheckRadius, m_groundLayer);

        m_justLanded = m_isGrounded && !m_wasGroundedLastFrame;

        if (m_justLanded)
        {
            OnLanded?.Invoke(); // used in AnimatorController to trigger "Land"
        }
    }

    void OnDrawGizmosSelected()
    {
        if (m_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_groundCheck.position, m_groundCheckRadius);
        }
    }

    // Public accessors for animation logic
    public float SpeedParameter
    {
        get
        {
            if (m_moveInput.x == 0) return 0f;
            return m_isRunning ? 1f : 0.5f;
        }
    }
}
