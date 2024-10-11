using Assets.Features.Fragments.ScriptableObjectEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Features.Systems
{
    public class ObjectSpawner: NetworkManager
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

       

        [Rpc(SendTo.Server)]
        private void OnSpawnObjects()
        {
            var index = Random.Range(0, networkObjectPrefabs.GetList().Count);
            var x = Random.Range(minDeviation.x, maxDeviation.x);
            var y = Random.Range(minDeviation.y, maxDeviation.y);
            var z = Random.Range(minDeviation.z, maxDeviation.z);
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(null);
        }
    }
}
