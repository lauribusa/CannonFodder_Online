using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using UnityEngine;

public class EventSOTesting : NetworkBehaviour
{
    [SerializeField] private VoidEventSO _onCannonFired;

    private void OnEnable() => _onCannonFired.Subscribe(OnCannonFired);

    private void OnDisable() => _onCannonFired.Unsubscribe(OnCannonFired);

    private void OnCannonFired() => OnCannonFiredClientRpc();

    [Rpc(SendTo.ClientsAndHost)]
    private void OnCannonFiredClientRpc()
    {
        Debug.Log("Cannon has fired");
    }
}