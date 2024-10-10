using Assets.Features.Fragments.ComponentEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class PlayerNetworkServer: NetworkBehaviour
    {
        public ItemPool carriableItemsInScene;
        public VoidEvent setItemParent;

        [Rpc(SendTo.Server)]
        public void SetItemParentServerSideRpc(sbyte id)
        {
            Debug.Log($"Asking item {id} from {gameObject.name}", gameObject);
            var item = carriableItemsInScene.Get(id);
            if (item == null) return;
            if (IsOwner)
            {
                GetComponent<PlayerNetworkClient>().SetItemParentRpc(id);
            }            
        }
    }
}
