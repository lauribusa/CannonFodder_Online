using UnityEngine;

namespace Assets.Features.Entities
{
    public class BillboardCanvas: MonoBehaviour
    {
        private Camera _main;
        private void Awake()
        {
            _main = Camera.main;
        }

        private void Update()
        {
            SetBillboard();
        }

        private void SetBillboard()
        {
            transform.rotation = _main.transform.rotation;
        }
    }
}
