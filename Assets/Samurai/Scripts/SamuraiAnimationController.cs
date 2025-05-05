using UnityEngine;
using UnityEngine.InputSystem;

public class SamuraiAnimationController : MonoBehaviour
{
    private Animator _animator;

    [Header("References")]
    [SerializeField] private SamuraiController mover;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (mover == null) return;

        // Update walk/run animation
        _animator.SetFloat("onWalk", mover.SpeedParameter);

        // Handle attack input (optional: you can move this to the mover too)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _animator.SetTrigger("onAttack_One");
        }
    }
}
