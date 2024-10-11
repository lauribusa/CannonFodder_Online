using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;

namespace Assets.Features.Systems
{
    public class ServerSideButton: NetworkBehaviour
    {
        public VoidEventSO @event;
        public void TriggerEvent()
        {
            if (IsServer) @event.Trigger();
        }
    }
}
