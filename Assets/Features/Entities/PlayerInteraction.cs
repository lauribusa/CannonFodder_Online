using Assets.Features.Entities;
using Assets.Features.Fragments.ScriptableObjectVariables;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    public event Action<sbyte> ItemLoadingRequested;
    public event Action ItemUnloadingRequested;

    [SerializeField] ItemPool _carriableItemsInScene;

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
        if (!IsLocalPlayer) return;
        if (!IsReadyToloadItem) return;

        if (_playerNetworkClient.carriedItem)
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            RequestToLoadItemServerRpc();
        }
        else
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            RequestToUnloadItemServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestToLoadItemServerRpc()
    {
        Item carriedItem = _playerNetworkClient.carriedItem;

        Debug.Log($"<color=yellow>Player is carrying: {_playerNetworkClient.carriedItem.name} ID: {_playerNetworkClient.carriedItem.Id.Value}</color>");
        _playerNetworkClient.PerformPutDownRpc();

        Debug.Log($"<color=yellow>BulletLoader has put down: {carriedItem.name} ID: {carriedItem.Id.Value}</color>");
        ItemLoadingRequested?.Invoke(carriedItem.Id.Value);
    }

    [Rpc(SendTo.Server)]
    private void RequestToUnloadItemServerRpc() => ItemUnloadingRequested?.Invoke();

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