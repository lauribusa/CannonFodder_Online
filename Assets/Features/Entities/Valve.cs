using Unity.Netcode;
using UnityEngine;

public class Valve : NetworkBehaviour
{
    [SerializeField] private float _turnSpeed = 5;
    [SerializeField] private Transform _valve;

    [SerializeField] private float _interpolateSpeed = 5;
    [SerializeField] private PositionInterpolator _interpolatedTransform;

    private void Update()
    {
        if (Input.GetKey(KeyCode.C)) TurnValve();

        if (Input.GetKey(KeyCode.V)) TurnValveReverse();
    }

    private void TurnValve() => TurnValveServerRpc();

    [Rpc(SendTo.Server)]
    private void TurnValveServerRpc() => TurnValveClientRpc();

    [Rpc(SendTo.ClientsAndHost)]
    private void TurnValveClientRpc()
    {
        RotateValve(-_turnSpeed);
        _interpolatedTransform.Interpolate(_interpolateSpeed);
    }

    private void TurnValveReverse() => TurnValveReverseServerRpc();

    [Rpc(SendTo.Server)]
    private void TurnValveReverseServerRpc() => TurnValveReverseClientRpc();

    [Rpc(SendTo.ClientsAndHost)]
    private void TurnValveReverseClientRpc()
    {
        RotateValve(_turnSpeed);
        _interpolatedTransform.InterpolateBack(_interpolateSpeed);
    }

    private void RotateValve(float turnSpeed)
    {
        Vector3 localrotation = _valve.localRotation.eulerAngles;
        localrotation.z += turnSpeed * Time.deltaTime;

        _valve.localRotation = Quaternion.Euler(localrotation);
    }
}