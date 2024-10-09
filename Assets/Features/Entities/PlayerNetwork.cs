using Assets.Features.Entities;
using Assets.Features.Fragments.ScriptableObjectEvents;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField]
    private NetworkServerSide server;
    [SerializeField]
    private NetworkVariable<Color> playerColor = new();
    [SerializeField]
    private NetworkVariable<FixedString128Bytes> playerName = new(writePerm: NetworkVariableWritePermission.Owner);
    [SerializeField]
    private TextMeshProUGUI _tag;

    public VoidEventSO onSingletonSpawned;
    public VoidEventSO onSingletonDestroyed;

    public override void OnNetworkSpawn()
    {

        playerColor.OnValueChanged += OnColorChange;
        playerName.OnValueChanged += OnTextChange;
        if (IsServer)
        {
            playerColor.Value = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        }
        if (IsLocalPlayer)
        {
            GetPositionRpc();
            playerName.Value = $"Player {NetworkManager.Singleton.LocalClientId+1}";
        }

        OnTextChange(playerName.Value, playerName.Value);
        OnColorChange(playerColor.Value, playerColor.Value);

        onSingletonSpawned.Trigger();
        Debug.Log($"Player spawned at {server.Time.Value} s");
    }

    public override void OnNetworkDespawn()
    {
        playerColor.OnValueChanged -= OnColorChange;
        playerName.OnValueChanged -= OnTextChange;
        onSingletonDestroyed.Trigger();
    }

    private void OnTextChange(FixedString128Bytes previous, FixedString128Bytes newValue)
    {
        gameObject.name = newValue.ToString();
        _tag.SetText(newValue.ToString());
    }

    private void OnColorChange(Color prev, Color next)
    {
        GetComponentInChildren<MeshRenderer>().material.color = next;
    }

    private void OnNumberChange(byte previous, byte next)
    {
        var newName = $"Player {next}";
        playerName.Value = newName;
    }

    [Rpc(SendTo.Server)]
    private void GetPositionRpc()
    {
        var playerPosition = new Vector3(Random.Range(transform.position.x - 1.5f, transform.position.x + 1.5f), transform.position.y, Random.Range(transform.position.z - 1.5f, transform.position.z + 1.5f));
        if (!IsHost) transform.position = playerPosition;
        SetPositionRpc(playerPosition);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPositionRpc(Vector3 position)
    {
        transform.position = position;
    }
}
