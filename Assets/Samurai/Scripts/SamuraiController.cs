using UnityEngine;
using PlayerActions;

[RequireComponent(typeof(Rigidbody2D))]
public class SamuraiController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visuals;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;

    private Rigidbody2D _rb;
    private PlayerInputActions _input;
    private Vector2 _moveInput;
    private bool _isRunning;

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
        float speed = _isRunning ? runSpeed : walkSpeed;
        Vector2 velocity = new Vector2(_moveInput.x * speed, _rb.linearVelocity.y);
        _rb.linearVelocity = velocity;
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
