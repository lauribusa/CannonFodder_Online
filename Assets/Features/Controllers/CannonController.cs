using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Controllers
{
    public class CannonController : NetworkBehaviour
    {
        [SerializeField] private Transform _rotator;
        [SerializeField] private Transform _elevator;

        [SerializeField] private BoolEventSO _BulletLoadedInCannon;
        [SerializeField] private VoidEventSO _onCannonFireRequested;
        [SerializeField] private VoidEventSO _onCannonFired;
        [SerializeField] private VoidEventSO _onScore;

        private bool _isBulletLoaded;
        private bool _isPowderLoaded;

        private void OnEnable()
        {
            _BulletLoadedInCannon.Subscribe(OnBulletInCannonChanged);
            _onCannonFireRequested.Subscribe(FireCannon);
        }

        private void OnDisable()
        {
            _BulletLoadedInCannon.Unsubscribe(OnBulletInCannonChanged);
            _onCannonFireRequested.Unsubscribe(FireCannon);
        }

        private void OnBulletInCannonChanged(bool isInCannon) => _isBulletLoaded = isInCannon;

        public void FireCannon()
        {
            if (!_isBulletLoaded) return;

            FireCannonServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void FireCannonServerRpc()
        {
            _isBulletLoaded = false;
            
            Debug.Log("Score!!!");
            FireCannonClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void FireCannonClientRpc()
        {
            //_onCannonFired.Trigger();
            _onScore.Trigger();
            Debug.Log("POW!!!");
        }
    }
}