using Assets.Features.Entities;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    public event Action<Item> ItemLoadingRequested;

    public bool IsReadyToloadItem { get; set; }

    private PlayerNetworkClient _playerNetworkClient;
    private Valve _valve;
    private FireCannonTrigger _cannonTrigger;

    private void Awake() => _playerNetworkClient = GetComponent<PlayerNetworkClient>();

    private void Update()
    {
        if (!IsLocalPlayer) return;

        RequestToLoadItem();
        TurnValve();
        PushTheButton();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer) return;
        if (other.TryGetComponent(out Valve valve))
        {
            _valve = valve;
        }

        if (other.TryGetComponent(out FireCannonTrigger cannonTrigger))
        {
            _cannonTrigger = cannonTrigger;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsLocalPlayer) return;
        if (other.TryGetComponent(out Valve valve))
        {
            _valve = null;
        }

        if (other.TryGetComponent(out FireCannonTrigger cannonTrigger))
        {
            _cannonTrigger = null;
        }
    }

    private void RequestToLoadItem()
    {
        if (!Input.GetKeyDown(KeyCode.L)) return;
        if (!IsReadyToloadItem) return;

        Item carriedItem = _playerNetworkClient.carriedItem;
        if (!carriedItem) return;

        _playerNetworkClient.PerformPutDownRpc();
        ItemLoadingRequested?.Invoke(carriedItem);
    }

    private void TurnValve()
    {
        if (!_valve) return;

        if (Input.GetKey(KeyCode.C)) _valve.TurnValve();
        if (Input.GetKey(KeyCode.V)) _valve.TurnValveReverse();
    }

    private void PushTheButton()
    {
        if (!_cannonTrigger) return;

        if (Input.GetKeyDown(KeyCode.L)) _cannonTrigger.PushTheButton();
    }
}