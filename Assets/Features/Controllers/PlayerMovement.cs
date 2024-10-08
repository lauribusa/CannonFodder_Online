using UnityEngine;

namespace Assets.Features.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0)] private float _moveSpeed = 10;
        [SerializeField, Min(0)] private float _rotateSpeed = 10;

        private Rigidbody _rigidbody;
        private Transform _camera;
        private Vector3 _inputDirection;
        private Vector3 _moveDirection;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _camera = Camera.main.transform;
        }

        private void Update()
        {
            SetInputDirection();
            AdaptDirectionToCamera();
            Rotate();
        }

        private void FixedUpdate() => Move();

        private void SetInputDirection()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            _inputDirection = new Vector3(horizontal, 0, vertical).normalized;
        }

        private void AdaptDirectionToCamera()
        {
            if (_inputDirection == Vector3.zero)
            {
                _moveDirection = Vector3.zero;
                return;
            }

            float targetAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            _moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }

        private void Move()
        {
            Vector3 velocity = _moveSpeed * _moveDirection;
            _rigidbody.AddForce(velocity);
        }

        private void Rotate()
        {
            if (_inputDirection == Vector3.zero) return;
            
            var lookRotation = Quaternion.LookRotation(_moveDirection);
            _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, lookRotation, _rotateSpeed * Time.deltaTime);
        }
    }
}