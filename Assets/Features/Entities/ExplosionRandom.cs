using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplosionRandom : NetworkBehaviour
{
    [SerializeField] private VoidEventSO _onScore;

    [SerializeField] private float _intensity = 3;

    private void OnEnable() => _onScore.Subscribe(OnScore);

    private void OnDisable() => _onScore.Unsubscribe(OnScore);

    private void OnScore() => ExplodeServerRpc(Random.insideUnitSphere);

    [Rpc(SendTo.Server)]
    private void ExplodeServerRpc(Vector3 position) => ExplodeClientRpc(position);

    [Rpc(SendTo.ClientsAndHost)]
    private void ExplodeClientRpc(Vector3 position)
    {
        GetComponent<Rigidbody>().AddForce(position * _intensity, ForceMode.Impulse);
    }
}