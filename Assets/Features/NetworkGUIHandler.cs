using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Assets.Features
{
    public class NetworkGUIHandler: MonoBehaviour
    {
        public VoidEventSO onStartHost;
        public VoidEventSO onStartClient;
        public VoidEventSO onShutdown;

        private string _ipAddress = "127.0.0.1";

        private void OnEnable()
        {
            onStartHost.Subscribe(StartHost);
            onStartClient.Subscribe(StartClient);
            onShutdown.Subscribe(Stop);
        }

        public void OnIpEntered(string ip)
        {
            Debug.Log($"Set {ip} address");
            _ipAddress = ip;
        }

        private void OnDisable()
        {
            onStartClient.Unsubscribe(StartClient);
            onStartHost.Unsubscribe(StartHost);
            onShutdown.Unsubscribe(Stop);
        }

        private void SetIp()
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(_ipAddress, 7777);
        }

        public void StartHost()
        {
            SetIp();
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            SetIp();
            NetworkManager.Singleton.StartClient();
        }

        public void Stop()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
