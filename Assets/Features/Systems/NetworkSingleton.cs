using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;

public class NetworkSingleton : NetworkBehaviour
{
    public NetworkVariable<byte> playerId = new(0);

    public static NetworkSingleton Instance { get; private set; }
    public NetworkVariable<int> Time = new();
    private float timer = 1;
    private NetworkVariable<bool> isRunning = new();
    public VoidEventSO onSingletonSpawned;

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
            SetInstance();
            onSingletonSpawned.Trigger();
        if (IsServer)
        {
            isRunning.Value = true;
        }
    }

    private void Update()
    {
        if (!IsServer || !isRunning.Value) return;
        timer -= UnityEngine.Time.deltaTime;
        if (timer <= 0)
        {
            SetTimeRpc();
            timer = 1;
        }
    }

    [Rpc(SendTo.Server)]
    public void IncreasePlayerCountRpc() => playerId.Value = (byte)(playerId.Value + 1);

    [Rpc(SendTo.Server)]
    public void DecreasePlayerCountRpc() => playerId.Value = (byte)(playerId.Value - 1);

    [Rpc(SendTo.Server)]
    public void SetTimeRpc() => Time.Value += 1;
}
