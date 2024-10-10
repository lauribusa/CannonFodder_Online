using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using UnityEngine;

public class PositionInterpolator : NetworkBehaviour
{
    [SerializeField] private Transform _transformDestination;
    [SerializeField] private Transform _startTransform;

    [SerializeField] private Transform _movingObject;

    [SerializeField] private VoidEventSO _onLoadingCarCompleted;
    [SerializeField] private VoidEventSO _onLoadingCarReseted;

    public bool IsComplete => _movingObject.position == _transformDestination.position;

    public bool IsAtStart => _movingObject.position == _startTransform.position;

    private void OnDrawGizmos()
    {
        if (!_transformDestination || !_startTransform) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_startTransform.position, _transformDestination.position);
        Gizmos.DrawSphere(_transformDestination.position, 0.1f);
    }

    public void Interpolate(float speed) => InterpolateServerRpc(speed);

    [Rpc(SendTo.Server)]
    private void InterpolateServerRpc(float speed) => InterpolateClientRpc(speed);

    [Rpc(SendTo.ClientsAndHost)]
    private void InterpolateClientRpc(float speed)
    {
        _movingObject.position = Vector3.MoveTowards(_movingObject.position, _transformDestination.position, speed * Time.deltaTime);

        if (!IsComplete) return;
        
        _onLoadingCarCompleted.Trigger();
    }

    public void InterpolateBack(float speed) => InterpolateBackServerRpc(speed);

    [Rpc(SendTo.Server)]
    private void InterpolateBackServerRpc(float speed) => InterpolateBackClientRpc(speed);

    [Rpc(SendTo.ClientsAndHost)]
    private void InterpolateBackClientRpc(float speed)
    {
        _movingObject.position = Vector3.MoveTowards(_movingObject.position, _startTransform.position, speed * Time.deltaTime);

        if (!IsAtStart) return;

        _onLoadingCarReseted.Trigger();
    }
}