using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features
{
    public class NetworkGUIHandler: MonoBehaviour
    {
        public VoidEventSO onStartHost;
        public VoidEventSO onStartClient;
        public VoidEventSO onShutdown;

        private void OnEnable()
        {
            onStartHost.Subscribe(StartHost);
            onStartClient.Subscribe(StartClient);
            onShutdown.Subscribe(Stop);
        }

        private void OnDisable()
        {
            onStartClient.Unsubscribe(StartClient);
            onStartHost.Unsubscribe(StartHost);
            onShutdown.Unsubscribe(Stop);
        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }

        public void Stop()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
