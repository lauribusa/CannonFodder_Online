using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using UnityEngine;

public class DebugScore : NetworkBehaviour
{
    public VoidEventSO onScore;

    public void SendScoreEvent()
    {
        SendScoreEventRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SendScoreEventRpc()
    {
        onScore.Trigger();
    }
}
