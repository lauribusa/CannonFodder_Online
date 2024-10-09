using Assets.Features.Fragments.ScriptableObjectEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class NetworkServerSide: NetworkBehaviour
    {
        public NetworkVariable<bool> isRunning = new();
        public NetworkVariable<byte> playerId = new(0);
        public NetworkVariable<int> Time = new();

        public ItemPool carriableItemsInScene;
        public VoidEventSO onSpawned;
        public VoidEventSO onDespawned;
        
        private float timer = 1;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                isRunning.Value = true;
            }
        }

        private void Update()
        {
            if (!IsServer || !isRunning.Value) return;
            timer -= UnityEngine.Time.deltaTime;
            if (timer <= 0)
            {
                SetTimeRpc();
                timer = 1;
            }
        }

        [Rpc(SendTo.Server)]
        public void IncreasePlayerCountRpc() => playerId.Value = (byte)(playerId.Value + 1);

        [Rpc(SendTo.Server)]
        public void DecreasePlayerCountRpc() => playerId.Value = (byte)(playerId.Value - 1);

        [Rpc(SendTo.Server)]
        public void SetTimeRpc() => Time.Value += 1;

        [Rpc(SendTo.Server)]
        public void SetItemParentServerSideRpc(sbyte id)
        {
            var item = carriableItemsInScene.Get(id);
            if (item == null) return;
            if (!IsHost)
            {
                GetComponent<PlayerActor>().SetItemParentRpc(id);
                return;
            }
            item.transform.localPosition = new Vector3(0, 1, 1);
        }
    }
}
