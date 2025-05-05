using PlayerActions;
using UnityEngine;
public class SamuraiAnimationController : MonoBehaviour
{
    private Animator _animator;
    private PlayerInputActions _input;

    [Header("References")]
    [SerializeField] private SamuraiController mover;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _input = new PlayerInputActions();
    }

    void OnEnable()
    {
        _input.Enable();

        mover.OnJumpStarted += () =>
        {
            _animator.SetTrigger("onJump");
        };

        _input.Player.Attack.performed += ctx =>
        {
            _animator.SetTrigger("onAttack_One");
        };

        mover.OnLanded += () =>
        {
            _animator.SetTrigger("onLand");
        };
    }

    void OnDisable()
    {
        _input.Disable();
        mover.OnJumpStarted -= () => { _animator.SetTrigger("onJump"); };
        mover.OnLanded -= () => { _animator.SetTrigger("onLand"); };

    }

    void Update()
    {
        if (mover == null) return;

        _animator.SetFloat("onWalk", mover.SpeedParameter);

        _animator.SetBool("onFall", mover.IsFalling);

        // Detect attack state
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(1);
        bool isAttacking = state.IsTag("Attack_01");

        mover.IsAttacking = isAttacking;
    }
}
