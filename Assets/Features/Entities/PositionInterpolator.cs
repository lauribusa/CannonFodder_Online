using System;
using Unity.Netcode;
using UnityEngine;

public class PositionInterpolator : NetworkBehaviour
{
    [SerializeField] private Transform _positionToMove;
    [SerializeField] private Transform _movingObject;

    public event Action Completed;
    public event Action MovedAtStart;

    public bool IsComplete => _movingObject.position == _positionToMove.position;

    public bool IsAtStart => _movingObject.position == _transform.position;

    private Transform _transform;

    private void Awake() => _transform = transform;

    private void OnDrawGizmos()
    {
        if (!_positionToMove) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _positionToMove.position);
        Gizmos.DrawSphere(_positionToMove.position, 0.1f);
    }

    public void Interpolate(float speed) => InterpolateServerRpc(speed);

    [Rpc(SendTo.Server)]
    private void InterpolateServerRpc(float speed) => InterpolateClientRpc(speed);

    [Rpc(SendTo.ClientsAndHost)]
    private void InterpolateClientRpc(float speed)
    {
        _movingObject.position = Vector3.MoveTowards(_movingObject.position, _positionToMove.position, speed * Time.deltaTime);

        if (IsComplete) return;

        Completed?.Invoke();
    }

    public void InterpolateBack(float speed) => InterpolateBackServerRpc(speed);

    [Rpc(SendTo.Server)]
    private void InterpolateBackServerRpc(float speed) => InterpolateBackClientRpc(speed);

    [Rpc(SendTo.ClientsAndHost)]
    private void InterpolateBackClientRpc(float speed)
    {
        _movingObject.position = Vector3.MoveTowards(_movingObject.position, _transform.position, speed * Time.deltaTime);

        if (IsAtStart) return;

        MovedAtStart?.Invoke();
    }
}