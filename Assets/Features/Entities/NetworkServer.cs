﻿using Assets.Features.Fragments.ScriptableObjectEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    internal class NetworkServer : NetworkBehaviour
    {
        public NetworkVariable<int> Time = new();
        public NetworkVariable<byte> playerId = new(0);

        private float timer = 1;

        public NetworkVariable<bool> isRunning = new();
        public FloatVariableSO timeSO;
        public VoidEventSO onPlayerSpawn;
        public VoidEventSO onPlayerLeave;

        private void OnEnable()
        {
            onPlayerSpawn.Subscribe(OnPlayerSpawn);
            onPlayerLeave.Subscribe(OnPlayerDespawn);
        }

        private void OnDisable()
        {
            onPlayerSpawn.Unsubscribe(OnPlayerSpawn);
            onPlayerLeave.Unsubscribe(OnPlayerDespawn);
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                isRunning.Value = true;
                playerId.OnValueChanged += OnPlayerCountChanged;
            }

            Time.OnValueChanged += OnTimeChanged;
            OnTimeChanged(Time.Value, Time.Value);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                playerId.OnValueChanged -= OnPlayerCountChanged;
            }
            Time.OnValueChanged -= OnTimeChanged;
        }

        [Rpc(SendTo.Server)]
        public void SetTimeRpc() => Time.Value += 1;

        private void Update()
        {
            if (!IsServer || !isRunning.Value) return;
            timer -= UnityEngine.Time.deltaTime;
            if (timer <= 0)
            {
                Debug.Log($"1sec");
                SetTimeRpc();
                timer = 1;
            }
        }

        private void OnTimeChanged(int prev, int next)
        {
            timeSO.Value = next;
        }

        private void OnPlayerSpawn()
        {
            IncreasePlayerCountRpc();
        }

        private void OnPlayerDespawn()
        {
            DecreasePlayerCountRpc();
        }

        [Rpc(SendTo.Server)]
        public void IncreasePlayerCountRpc() => playerId.Value = (byte)(playerId.Value + 1);

        [Rpc(SendTo.Server)]
        public void DecreasePlayerCountRpc() => playerId.Value = (byte)(playerId.Value - 1);

        private void OnPlayerCountChanged(byte prev, byte next)
        {
            Debug.Log($"PlayerCount: {next}");
        }

    }
}
