using Assets.Features.Entities;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerItemLoader : MonoBehaviour
{
    public event Action<Item> ItemLoadingRequested;

    public bool IsReadyToloadItem { get; set; }

    private PlayerNetworkClient _playerActor;

    private void Awake() => _playerActor = GetComponent<PlayerNetworkClient>();

    private void Update()
    {
        //if (!IsLocalPlayer) return;

        if (!Input.GetKeyDown(KeyCode.L)) return;
        Debug.Log("Press L");

        if (!IsReadyToloadItem) return;
        Debug.Log($"Player ready to load: {IsReadyToloadItem}");

        if (!_playerActor.carriedItem) return;
        Debug.Log($"Player carried item: {_playerActor.carriedItem.name}");

        ItemLoadingRequested?.Invoke(_playerActor.carriedItem);
    }
}