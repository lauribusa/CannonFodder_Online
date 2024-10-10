using Assets.Features.Entities;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerItemLoader : NetworkBehaviour
{
    public event Action<Item> ItemLoadingRequested;

    public bool IsReadyToloadItem { get; set; }

    private PlayerNetworkClient _playerActor;

    private void Awake() => _playerActor = GetComponent<PlayerNetworkClient>();

    private void Update()
    {
        if (!IsLocalPlayer) return;
        if (!Input.GetKeyDown(KeyCode.L)) return;
        if (!IsReadyToloadItem) return;
        if (!_playerActor.carriedItem) return;

        ItemLoadingRequested?.Invoke(_playerActor.carriedItem);
    }
}