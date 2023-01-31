using System.Collections;
using Cinemachine.Utility;
using DG.Tweening;
using Pilk.Scripts.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pilk.Scripts
{
    public class PlatformingCharacterController : MonoBehaviour
    {
        [SerializeField, Required]
        private Rigidbody _physicsBody;
        [SerializeField, Required]
        private PlayerStatsData _playerStats;
        [SerializeField, Required]
        private Camera _camera;
        [SerializeField, Required]
        private Animator _animator;
        private AnimationController _animationController;

        [Space]
        [SerializeField, FoldoutGroup("Settings"), Range(1f, 10f)]
        private float _turningSpeed;
        [SerializeField, FoldoutGroup("Settings")]
        private float _movementSpeedThreshold = 0.1f;
        [SerializeField, FoldoutGroup("Settings")]
        private LayerMask _groundMask;

        [Space]
        [ShowInInspector, FoldoutGroup("Info"), ReadOnly]
        private Vector3 _horizontalVelocity;
        [ShowInInspector, FoldoutGroup("Info"), ReadOnly]
        private Vector3 _verticalVelocity;
        [ShowInInspector, FoldoutGroup("Info"), ReadOnly]
        private bool _inputLocked;
        [ShowInInspector, FoldoutGroup("Info"), ReadOnly]
        private Vector2 _movementInputs;
        private Vector3 _desiredMovementDirection;
        
        private bool _jumpInput;
        private bool _attackInput;
        private bool _attackEnabled = true;

        void Start()
        {
            Cursor.lockState     = CursorLockMode.Locked;
            Cursor.visible       = false;
            _camera              = Camera.main;
            _animator            = GetComponent<Animator>();
            _animationController = new AnimationController(_animator, .3f);
        }

        void Update()
        {
            TryPerformJump();
            ApplyGravity(Time.deltaTime);
            UpdateVelocity(Time.deltaTime);
            LookAtMovementDirection(Time.deltaTime);
            TryPerformAttack();
        }

        private void ApplyGravity(float deltaTime)
        {
            _verticalVelocity.y += -2 * _playerStats.JumpHeight * Mathf.Pow(_playerStats.MovementSpeed, 2) * deltaTime
                                   / Mathf.Pow(_playerStats.JumpDistance, 2);
            _verticalVelocity.y = Mathf.Clamp(_verticalVelocity.y, CheckGround() ? -.05f : -_playerStats.MaxFallSpeed, 10f);
        }

        private void UpdateVelocity(float deltaTime)
        {
            if (_inputLocked)
            { 
                bool customDirection = _desiredMovementDirection != Vector3.zero;
                _horizontalVelocity = (customDirection ? _desiredMovementDirection : transform.forward);
                _verticalVelocity = Vector3.zero;
                _physicsBody.velocity = _horizontalVelocity * _playerStats.AttackSpeed + _verticalVelocity;
            }
            else
            {
                TurnToDesiredDirection(deltaTime);
                StopIfSlowEnough();
                _physicsBody.velocity = _horizontalVelocity * _playerStats.MovementSpeed + _verticalVelocity;
            }
            
            if (_verticalVelocity.magnitude > 0.1f)
                return;
            _animationController.SwitchAnimation(_horizontalVelocity.magnitude > _movementSpeedThreshold ? 
                                                 AnimationController.Walk :
                                                 AnimationController.Idle);
        }

        public void SetVerticalVelocity(float velocity)
        {
            _verticalVelocity.y = velocity;
        }

        #region Horizontal movement related region
        private void TurnToDesiredDirection(float deltaTime)
        {
            _desiredMovementDirection = GetCameraForward() * _movementInputs.y + GetCameraRight() * _movementInputs.x;
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, _desiredMovementDirection, deltaTime * _turningSpeed);
        }

        private void StopIfSlowEnough()
        {
            if (_horizontalVelocity.magnitude < _movementSpeedThreshold && _movementInputs.magnitude < 0.1f)
                _horizontalVelocity = Vector3.zero;
        }

        private void LookAtMovementDirection(float deltaTime)
        {
            if (_horizontalVelocity.magnitude > 0)
                transform.DOLookAt(transform.position + _horizontalVelocity, deltaTime * _turningSpeed);
        }
        #endregion

        #region Attack related region
        private void TryPerformAttack()
        {
            if (_attackInput && _attackEnabled)
            {
                _attackInput = false;
                var lockTime = _playerStats.AttackDistance / _playerStats.AttackSpeed;
                var switched = _animationController.SwitchAnimation(AnimationController.Attack);
                if (switched)
                    StartCoroutine(_animationController.LockAnimation(lockTime));
                StartCoroutine(LockInput(lockTime));
                StartCoroutine(CooldownAttack(2f));
            }
        }
        
        private IEnumerator CooldownAttack(float duration)
        {
            _attackEnabled = false;
            yield return new WaitForSeconds(duration);
            _attackEnabled = true;
        }
        #endregion
        
        #region Jump related region
        private void TryPerformJump()
        {
            bool canJump = _jumpInput && CheckGround();
            if (canJump)
            {
                _verticalVelocity.y = 2 * _playerStats.JumpHeight * 
                                      _playerStats.MovementSpeed * _playerStats.JumpDistance;
                _animationController.SwitchAnimation(AnimationController.Jump);
            }
        }

        private bool CheckGround()
        {
            var grounded = Physics.Raycast(transform.position + Vector3.up * .04f, 
                        Vector3.down, .05f, _groundMask);
            return grounded;
        }
        #endregion

        #region Camera specific region
        private Vector3 GetCameraForward()
        {
            var vector = _camera.transform.forward;
            return vector.ProjectOntoPlane(Vector3.up);
        }

        private Vector3 GetCameraRight()
        {
            var vector = _camera.transform.right;
            return vector.ProjectOntoPlane(Vector3.up);
        }
        #endregion

        #region Inputs reading region
        private IEnumerator LockInput(float duration)
        {
            _inputLocked = true;
            yield return new WaitForSeconds(duration);
            _inputLocked = false;
        }
        
        public void ReadMovementInputs(InputAction.CallbackContext context)
        {
            _movementInputs = context.ReadValue<Vector2>();
        }

        public void ReadJumpInput(InputAction.CallbackContext context)
        {
            if (context.performed)
                return;
            _jumpInput = context.started;
        }

        public void ReadAttackInput(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            if (!_attackEnabled)
                return;
            _attackInput = true;
        }
        #endregion
    }
}
