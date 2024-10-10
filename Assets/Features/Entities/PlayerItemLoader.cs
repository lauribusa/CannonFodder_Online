using Assets.Features.Entities;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerItemLoader : NetworkBehaviour
{
    public event Action<Item> ItemLoadingRequested;

    public bool IsReadyToloadItem { get; set; }

    private PlayerNetworkClient _playerActor;
    private Valve _valve;

    private void Awake() => _playerActor = GetComponent<PlayerNetworkClient>();

    private void Update()
    {
        if (!IsLocalPlayer) return;

        RequestToLoadItem();
        TurnValve();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer) return;
        if (!other.TryGetComponent(out Valve valve)) return;

        _valve = valve;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsLocalPlayer) return;
        if (!other.TryGetComponent(out Valve valve)) return;

        _valve = null;
    }

    private void RequestToLoadItem()
    {
        if (!Input.GetKeyDown(KeyCode.L)) return;
        if (!IsReadyToloadItem) return;
        if (!_playerActor.carriedItem) return;

        ItemLoadingRequested?.Invoke(_playerActor.carriedItem);
    }

    private void TurnValve()
    {
        if (!_valve) return;

        if (Input.GetKey(KeyCode.C)) _valve.TurnValve();
        if (Input.GetKey(KeyCode.V)) _valve.TurnValveReverse();
    }
}