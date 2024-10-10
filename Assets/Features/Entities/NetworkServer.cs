using Assets.Features.Fragments.ScriptableObjectEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    internal class NetworkServer : NetworkBehaviour
    {
        public NetworkVariable<int> Time = new(120);
        public NetworkVariable<byte> playerId = new(0);
        public NetworkVariable<bool> isRunning = new();

        public FloatVariableSO timeSO;

        private float timer = 1;

        public VoidEventSO onPlayerSpawn;
        public VoidEventSO onPlayerLeave;

        public VoidEventSO onGameEnd;
        public VoidEventSO onGameStart;

        private void OnEnable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        public void OnClientConnected(ulong id)
        {
            
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                isRunning.Value = true;
                onPlayerSpawn.Subscribe(OnPlayerSpawn);
                onPlayerLeave.Subscribe(OnPlayerDespawn);
                playerId.OnValueChanged += OnPlayerCountChanged;
            }

            Time.OnValueChanged += OnTimeChanged;
            OnTimeChanged(Time.Value, Time.Value);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                onPlayerSpawn.Unsubscribe(OnPlayerSpawn);
                onPlayerLeave.Unsubscribe(OnPlayerDespawn);
                playerId.OnValueChanged -= OnPlayerCountChanged;
            }

            Time.OnValueChanged -= OnTimeChanged;
        }

        [Rpc(SendTo.Server)]
        public void SetTimeRpc()
        {
            Time.Value -= 1;
         }

        private void Update()
        {
            if (!IsServer) return;
            if (!isRunning.Value) return;
            timer -= UnityEngine.Time.deltaTime;
            if (timer <= 0)
            {
                SetTimeRpc();
                timer = 1;
            }
        }

        private void OnTimeChanged(int prev, int next)
        {
            var i = next < 0 ? 0 : next;
            if(next < 0)
            {
                Time.Value = i;
            }
            timeSO.Value = i;
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

        private void OnGameStart()
        {

        }

        private void OnGameEnd()
        {

        }

    }
}
