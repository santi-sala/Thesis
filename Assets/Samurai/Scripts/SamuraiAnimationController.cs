using PlayerActions;
using UnityEngine;
public class SamuraiAnimationController : MonoBehaviour
{
    private Animator m_animator;
    private PlayerInputActions m_input;
    private SamuraiController m_samuraiController;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_input = new PlayerInputActions();
        m_samuraiController = GetComponent<SamuraiController>();
    }

    void OnEnable()
    {
        m_input.Enable();

        m_samuraiController.OnJumpStarted += () =>
        {
            m_animator.SetTrigger("onJump");
        };

        m_input.Player.Attack.performed += ctx =>
        {
            m_animator.SetTrigger("onAttack_One");
        };

        m_samuraiController.OnLanded += () =>
        {
            m_animator.SetTrigger("onLand");
        };
    }

    void OnDisable()
    {
        m_input.Disable();
        m_samuraiController.OnJumpStarted -= () => { m_animator.SetTrigger("onJump"); };
        m_samuraiController.OnLanded -= () => { m_animator.SetTrigger("onLand"); };
    }

    void Update()
    {
        if (m_samuraiController == null) return;

        m_animator.SetFloat("onWalk", m_samuraiController.SpeedParameter);

        m_animator.SetBool("onFall", m_samuraiController.IsFalling);

        // Detect attack state
        AnimatorStateInfo state = m_animator.GetCurrentAnimatorStateInfo(1);
        bool isAttacking = state.IsTag("Attack_01");

        m_samuraiController.IsAttacking = isAttacking;
    }
}
