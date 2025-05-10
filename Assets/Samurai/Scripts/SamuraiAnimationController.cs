using PlayerActions;
using UnityEngine;
public class SamuraiAnimationController : MonoBehaviour
{
    private Animator _animator;
    private PlayerInputActions _input;
    private SamuraiController _samuraiController;

    

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _input = new PlayerInputActions();
        _samuraiController = GetComponent<SamuraiController>();
    }

    void OnEnable()
    {
        _input.Enable();

        _samuraiController.OnJumpStarted += () =>
        {
            _animator.SetTrigger("onJump");
        };

        _input.Player.Attack.performed += ctx =>
        {
            _animator.SetTrigger("onAttack_One");
        };

        _samuraiController.OnLanded += () =>
        {
            _animator.SetTrigger("onLand");
        };
    }

    void OnDisable()
    {
        _input.Disable();
        _samuraiController.OnJumpStarted -= () => { _animator.SetTrigger("onJump"); };
        _samuraiController.OnLanded -= () => { _animator.SetTrigger("onLand"); };

    }

    void Update()
    {
        if (_samuraiController == null) return;

        _animator.SetFloat("onWalk", _samuraiController.SpeedParameter);

        _animator.SetBool("onFall", _samuraiController.IsFalling);

        // Detect attack state
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(1);
        bool isAttacking = state.IsTag("Attack_01");

        _samuraiController.IsAttacking = isAttacking;
    }
}
