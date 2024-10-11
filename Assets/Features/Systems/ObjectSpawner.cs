using Assets.Features.Fragments.ScriptableObjectEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Systems
{
    public class ObjectSpawner: NetworkBehaviour
    {
        [SerializeField]
        private GameObjectListSO networkObjectPrefabs;
        [SerializeField]
        private VoidEventSO onSpawnObjects;
        [SerializeField]
        private Transform relativeSpawnPoint;
        [SerializeField]
        private Vector3 minDeviation;
        [SerializeField]
        private Vector3 maxDeviation;

        private void OnSpawnObject()
        {
            if (!IsServer) return;
            OnSpawnObjectsRpc();
        }

        [Rpc(SendTo.Server)]
        private void OnSpawnObjectsRpc()
        {
            foreach (var prefab in networkObjectPrefabs.GetList())
            {
                if (!prefab.TryGetComponent<NetworkObject>(out var networkObject)) continue;
                var index = Random.Range(0, networkObjectPrefabs.GetList().Count);
                var x = Random.Range(minDeviation.x, maxDeviation.x);
                var y = Random.Range(minDeviation.y, maxDeviation.y);
                var z = Random.Range(minDeviation.z, maxDeviation.z);
                NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(networkObject, position: new Vector3(x, y, z), rotation: Quaternion.identity);
            }
        }
    }
}
