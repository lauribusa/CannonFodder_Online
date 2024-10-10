using Assets.Features.Fragments.ComponentEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class PlayerNetworkServer: NetworkBehaviour
    {
        public ItemPool carriableItemsInScene;

        [Rpc(SendTo.Server)]
        public void SetItemParentServerSideRpc(sbyte id)
        {
            var item = carriableItemsInScene.Get(id);
            if (item == null) return;
            if (TryGetComponent<PlayerNetworkClient>(out var client))
            {
                if (item == null) return;
                if (IsLocalPlayer) Debug.Log($"LOCALPLAYER: ASSIGNING {id} ({item.name}) TO PLAYER {gameObject.name}");
              
                item.transform.SetParent(client.transform);
                client.SetItemParentRpc(id);
            }
            
            if (IsOwner) Debug.Log($"OWNER: ASSIGNING {id} ({item.name}) TO PLAYER {gameObject.name}");
            if (IsClient) Debug.Log(($"CLIENT: ASSIGNING {id} ({item.name}) TO PLAYER {gameObject.name}"));
            if (IsServer) Debug.Log($"SERVER: ASSIGNING {id} ({item.name}) TO PLAYER {gameObject.name}");
            if (!IsLocalPlayer) return;
        }
    }
}
