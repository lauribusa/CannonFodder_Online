using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using UnityEngine;

public class FireCannonTrigger : NetworkBehaviour
{
    [SerializeField] private VoidEventSO _onCannonFireRequested;

    public void PushTheButton() => PushTheButtonClientRpc();

    [Rpc(SendTo.ClientsAndHost)]
    public void PushTheButtonClientRpc() => _onCannonFireRequested.Trigger();
}