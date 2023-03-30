using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTutorial.Manager;

namespace UnityTutorial.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float AnimBlendSpeed = 1.0f;

        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 2109f;

        private Rigidbody _playerRigidbody;

        private InputManager _inputManager;

        private Animator _animator;

        private bool _hasAnimator;
        private int _xVelHash;
        private int _yVelHash;
        private int _jumpHash;
        private int _groundHash;
        private int _fallingHash;


        private const float _walkSpeed = 2f;
        private const float _runSpeed = 6f;

        private float _xRotation;

        private Vector2 _currentVelocity;
       private void Start()
        {
            _hasAnimator = TryGetComponent<Animator>(out _animator);
            _playerRigidbody= GetComponent<Rigidbody>();
            _inputManager= GetComponent<InputManager>();

            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");

        }

        private void FixedUpdate()
        {
            Move();
            HandleJump();
        }
        private void LateUpdate()
        {
            CamMovements();
        }
        private void Move()
        {
            if(!_hasAnimator) return;
            float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;
            if (_inputManager.Move == Vector2.zero) targetSpeed = 0.1f;

            _currentVelocity.x = targetSpeed * _inputManager.Move.x;
            _currentVelocity.y = targetSpeed * _inputManager.Move.y;

            var xVelDifference = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
            var zVelDifference = Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

            _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);

            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        private void CamMovements()
        {if(!_hasAnimator) return;
            var Mouse_X = _inputManager.Look.x;
            var Mouse_Y = _inputManager.Look.y;

            Camera.position = CameraRoot.position;

            _xRotation -= Mouse_Y * MouseSensitivity * Time.deltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));

        }
        private void HandleJump()
        {
            if(!_hasAnimator) return;
            if(!_inputManager.Jump) return;
            _animator.SetTrigger(_jumpHash);
        }    
    }
}
