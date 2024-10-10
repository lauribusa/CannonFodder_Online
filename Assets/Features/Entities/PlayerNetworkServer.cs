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
            if (!IsHost)
            {
                GetComponent<PlayerNetworkClient>().SetItemParentRpc(id);
                return;
            }
            item.transform.localPosition = new Vector3(0, 1, 1);
        }
    }
}
