using Assets.Features.Fragments.ScriptableObjectEvents;
using UnityEngine;

namespace Assets.Features.Controllers
{
    public class CannonController : MonoBehaviour
    {
        [SerializeField] private Transform _rotator;
        [SerializeField] private Transform _elevator;

        [SerializeField] private BoolEventSO _onBulletLoaded;

        public void RotateCannon(float degrees)
        {

        }
    }
}