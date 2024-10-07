using UnityEngine;

namespace Assets.Features.Controllers
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0)] private float _moveSpeed = 10;
        [SerializeField, Min(0)] private float _rotateSpeed = 10;

        private Rigidbody _rigidbody;
        private Vector3 _inputDirection;

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void Update()
        {
            SetInputDirection();
            Rotate();
        }

        private void FixedUpdate() => Move();

        private void SetInputDirection()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            _inputDirection.x = horizontal;
            _inputDirection.z = vertical;
            _inputDirection.Normalize();
        }

        private void Move()
        {
            Vector3 velocity = _moveSpeed * _inputDirection;
            _rigidbody.AddForce(velocity);
        }

        private void Rotate()
        {
            if (_inputDirection == Vector3.zero) return;
            
            var lookRotation = Quaternion.LookRotation(_inputDirection);
            _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, lookRotation, _rotateSpeed * Time.deltaTime);
        }
    }
}