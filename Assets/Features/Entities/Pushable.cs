using Assets.Features.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pushable : NetworkBehaviour
    {
        private Rigidbody _rigidbody;

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void OnCollisionStay(Collision collision) => PushedBy(collision);

        public void PushedBy(Collision collision)
        {
            if (!IsServer) return;
            if (!collision.gameObject.TryGetComponent(out PlayerMovement playerMovement)) return;

            Vector3 direction = collision.GetContact(0).normal.normalized;
            direction.y = 0;
            Vector3 forceDirection = direction * playerMovement.PushingForce;
            HandleCollisionServerRpc(forceDirection);
        }

        [Rpc(SendTo.Server)]
        private void HandleCollisionServerRpc(Vector3 forceDirection) =>
            ApplyForceClientRpc(forceDirection);

        [Rpc(SendTo.ClientsAndHost)]
        private void ApplyForceClientRpc(Vector3 forceDirection) =>
            _rigidbody.AddForce(forceDirection);
    }
}