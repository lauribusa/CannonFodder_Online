using Assets.Features.Entities;
using Assets.Features.Fragments.ScriptableObjectVariables;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerItemLoader : NetworkBehaviour
{
    public event Action<Item> ItemLoadingRequested;

    public ItemPool CarriableItemsInScene => _playerActor.carriableItemsInScene;
    public bool IsReadyToloadItem { get; set; }

    private PlayerActor _playerActor;

    private void Awake() => _playerActor = GetComponent<PlayerActor>();

    private void Update()
    {
        if (!IsLocalPlayer) return;
        if (!Input.GetKeyDown(KeyCode.L)) return;
        if (!IsReadyToloadItem) return;
        if (!_playerActor.carriedItem) return;

        ItemLoadingRequested?.Invoke(_playerActor.carriedItem);
    }
}