using Assets.Features.Entities;
using Assets.Features.Fragments.ScriptableObjectVariables;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    public event Action<sbyte> ItemLoadingRequested;

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
        if (!Input.GetKeyDown(KeyCode.L)) return;
        if (!IsReadyToloadItem) return;

        RequestToLoadItemServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void RequestToLoadItemServerRpc()
    {
        Item carriedItem = _playerNetworkClient.carriedItem;
        if (!carriedItem) return;

        Debug.Log($"<color=yellow>Player is carrying: {_playerNetworkClient.carriedItem.name} ID: {_playerNetworkClient.carriedItem.Id.Value}</color>");
        _playerNetworkClient.PerformPutDownRpc();

        Debug.Log($"<color=yellow>BulletLoader has put down: {carriedItem.name} ID: {carriedItem.Id.Value}</color>");
        ItemLoadingRequested?.Invoke(carriedItem.Id.Value);
        return;
        RequestToLoadItemClientRpc(carriedItem.Id.Value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RequestToLoadItemClientRpc(sbyte itemID)
    {
        //Item carriedItem = _playerNetworkClient.carriedItem;
        //if (!carriedItem) return;

        //Debug.Log($"<color=yellow>Player is carrying: {_playerNetworkClient.carriedItem.name} ID: {_playerNetworkClient.carriedItem.Id.Value}</color>");
        //_playerNetworkClient.PerformPutDownRpc();

        Item carriedItem = _carriableItemsInScene.Get(itemID);
        Debug.Log($"<color=yellow>BulletLoader has put down: {carriedItem.name} ID: {carriedItem.Id.Value}</color>");
        ItemLoadingRequested?.Invoke(itemID);
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