using Assets.Features.Fragments.ScriptableObjectEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    internal class NetworkServer : NetworkBehaviour
    {
        public NetworkVariable<byte> Score = new(0);
        public NetworkVariable<int> Time = new(120);
        public NetworkVariable<byte> playerId = new(0);
        public NetworkVariable<bool> isRunning = new();

        public NetworkVariable<byte[]> carriableItemIds = new();

        public FloatVariableSO timeSO;
        public FloatVariableSO scoreSO;

        private float timer = 1;

        public VoidEventSO onPlayerSpawn;
        public VoidEventSO onPlayerLeave;

        public VoidEventSO onGameEnd;
        public VoidEventSO onGameStart;

        public VoidEventSO onScored;

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
                onGameStart.Subscribe(OnGameStart);
                playerId.OnValueChanged += OnPlayerCountChanged;
            }

            onScored.Subscribe(OnPlayerScore);
            Time.OnValueChanged += OnTimeChanged;
            OnTimeChanged(Time.Value, Time.Value);
            Score.OnValueChanged += OnScoreChanged;
            OnScoreChanged(Score.Value, Score.Value);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                onPlayerSpawn.Unsubscribe(OnPlayerSpawn);
                onPlayerLeave.Unsubscribe(OnPlayerDespawn);
                onGameStart.Unsubscribe(OnGameStart);
                playerId.OnValueChanged -= OnPlayerCountChanged;
            }

            onScored.Unsubscribe(OnPlayerScore);
            Time.OnValueChanged -= OnTimeChanged;
            Score.OnValueChanged -= OnScoreChanged;
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
            if(next <= 0 && IsServer)
            {
                OnGameEnd();
            }
        }

        private void OnScoreChanged(byte prev,  byte next)
        {
            scoreSO.Value = next;
        }

        private void OnPlayerSpawn()
        {
            IncreasePlayerCountRpc();
        }

        private void OnPlayerDespawn()
        {
            DecreasePlayerCountRpc();
        }

        private void OnPlayerScore()
        {
            OnScoreRpc();
        }

        [Rpc(SendTo.Server)]
        public void OnScoreRpc()
        {
            byte i = (byte)(Score.Value + 1 > byte.MaxValue ? byte.MaxValue : Score.Value + 1);
            Score.Value = i;
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
            OnGameStartRpc();
        }

        [Rpc(SendTo.Server)]
        private void OnGameStartRpc()
        {
            if(!IsServer) return;
            isRunning.Value = true;
        }

        private void OnGameEnd()
        {
            onGameEnd.Trigger();
            OnGameEndRpc();
        }

        [Rpc(SendTo.Server)]
        private void OnGameEndRpc()
        {
            //if (!IsServer) return;
            Debug.Log($"Game end");
            isRunning.Value = false;
            Time.Value = 120;
            Score.Value = 0;
        }
    }
}
