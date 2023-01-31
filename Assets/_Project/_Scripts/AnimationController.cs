using System.Collections;
using UnityEngine;

namespace Pilk.Scripts
{
    public class AnimationController
    {
        public AnimationController(Animator animator, float transitionDuration)
        {
            _animator = animator;
            _transitionDuration = transitionDuration;
        }
        
        private readonly Animator _animator;
        private readonly float _transitionDuration;
        private int _currentAnimation;
        private bool _animationLocked;

        public static int Idle { get; } = Animator.StringToHash("Idle");
        public static int Walk { get; } = Animator.StringToHash("Walk");
        public static int Jump { get; } = Animator.StringToHash("Idle");
        public static int Attack { get; } = Animator.StringToHash("Attack");

        public bool SwitchAnimation(int animationId)
        {
            if (_currentAnimation == animationId)
                return false;
            
            if (_animationLocked)
                return false;
            
            _currentAnimation = animationId;
            _animator.CrossFade(_currentAnimation, _transitionDuration, 0, 0);
            return true;
        }

        public IEnumerator LockAnimation(float duration)
        {
            _animationLocked = true;
            yield return new WaitForSeconds(duration);
            _animationLocked = false;
        }
    }
}
