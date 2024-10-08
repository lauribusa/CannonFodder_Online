using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<Color> playerColor = new();
    private NetworkVariable<FixedString128Bytes> playerName = new(writePerm: NetworkVariableWritePermission.Server);
    [SerializeField]
    private TextMeshProUGUI _tag;
    private NetworkVariable<int> playerNumber = new(1, writePerm: NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        playerColor.OnValueChanged += OnColorChange;
        playerName.OnValueChanged += OnTextChange;

        if (IsLocalPlayer)
        {
            GetPositionRpc();
            GetNameRpc();
        }
        if (IsServer)
        {
            playerColor.Value = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
        }
        OnTextChange(playerName.Value, playerName.Value);
        OnColorChange(playerColor.Value, playerColor.Value);
    }

    public override void OnNetworkDespawn()
    {
        playerColor.OnValueChanged -= OnColorChange;
        playerName.OnValueChanged -= OnTextChange;
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

    [Rpc(SendTo.Server)]
    private void GetPositionRpc()
    {
        var playerPosition = new Vector3(UnityEngine.Random.Range(transform.position.x - 1.5f, transform.position.x + 1.5f), transform.position.y, UnityEngine.Random.Range(transform.position.z - 1.5f, transform.position.z + 1.5f));

        if (!IsHost) transform.position = playerPosition;

        SetPositionRpc(playerPosition);
    }

    [Rpc(SendTo.Server)]
    private void GetNameRpc()
    {
        var newName = $"Player {NetworkSingleton.Instance.playerId.Value}";
        playerName.Value = newName;
        NetworkSingleton.Instance.IncreaseRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetNameRpc()
    {

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPositionRpc(Vector3 position)
    {
        transform.position = position;
    }
}
