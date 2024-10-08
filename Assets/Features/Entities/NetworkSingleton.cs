using Unity.Netcode;

public class NetworkSingleton : NetworkBehaviour
{
    public NetworkVariable<int> playerId = new(1);

    public static NetworkSingleton Instance { get; private set; }

    private void Awake()
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

    [Rpc(SendTo.Server)]
    public void IncreaseRpc()
    {
        playerId.Value = playerId.Value + 1;
    }

    [Rpc(SendTo.Server)]
    public void DecreaseRpc()
    {
        playerId.Value = playerId.Value - 1;
    }
}
