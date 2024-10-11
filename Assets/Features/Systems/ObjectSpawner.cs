using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Systems
{
    public class ObjectSpawner: NetworkBehaviour
    {
        [SerializeField]
        private NetworkPrefabsList networkObjectPrefabs;
        [SerializeField]
        private VoidEventSO onSpawnObjects;
        [SerializeField]
        private Vector3 minDeviation;
        [SerializeField]
        private Vector3 maxDeviation;

        public override void OnNetworkSpawn()
        {
            onSpawnObjects.Subscribe(OnSpawnObjects);
        }

        public override void OnNetworkDespawn()
        {
            onSpawnObjects.Unsubscribe(OnSpawnObjects);
        }

        private void OnSpawnObjects()
        {
            if (!IsServer) return;
            OnSpawnObjectsRpc();
        }

        [Rpc(SendTo.Server)]
        private void OnSpawnObjectsRpc()
        {
            foreach (var prefab in networkObjectPrefabs.PrefabList)
            {
                if (!prefab.Prefab.TryGetComponent<NetworkObject>(out var networkObject)) continue;
                var index = Random.Range(0, networkObjectPrefabs.PrefabList.Count);
                var x = Random.Range(minDeviation.x, maxDeviation.x);
                var y = Random.Range(minDeviation.y, maxDeviation.y);
                var z = Random.Range(minDeviation.z, maxDeviation.z);
                NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(networkObject, position: transform.position + new Vector3(x, y, z), rotation: Quaternion.identity);
            }
        }
    }
}
