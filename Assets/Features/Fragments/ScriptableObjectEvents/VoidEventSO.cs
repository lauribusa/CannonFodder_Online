using Assets.Features.Fragments.Base;
using UnityEngine;

namespace Assets.Features.Fragments.ScriptableObjectEvents
{
    [CreateAssetMenu(menuName = "Events/Void")]
    public class VoidEventSO : ScriptableObjectEvent<Void>
    {
        public void Trigger()
        {
            Trigger(null);
        }
    }
}
