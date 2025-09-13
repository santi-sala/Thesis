using System.Collections.Generic;
using UnityEngine;

public class RestartAnimations : MonoBehaviour
{
    [SerializeField]
    private List<Animator> _animators;
    [SerializeField]
    private List<Animation> _animations;
    [SerializeField]
    private List<BallAnimator> _ballAnimations;
    [SerializeField]
    private TimeDisplay _timeDisplay;

    public void RestartAllAnimations()
    {
        foreach (var animator in _animators)
        {
            if (animator != null)
            {
                animator.Play(0, -1, 0f);
            }
        }

        foreach (var animation in _animations)
        {
            if (animation != null && animation.clip != null)
            {
                animation.Stop();
                animation.Play();
            }
        }

        foreach (var ballAnimation in _ballAnimations)
        {
            if (ballAnimation != null)
            {
                ballAnimation.RestartAnimation();
            }
        }
        if (_timeDisplay != null)
        {
            _timeDisplay.ResetTimer();
        }
    }
}
