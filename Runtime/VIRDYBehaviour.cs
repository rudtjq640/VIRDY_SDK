using UnityEngine;

namespace VIRDY.SDK
{
    public abstract class VIRDYBehaviour : MonoBehaviour
    {
        protected bool IsInitalized { get; private set; }

        private Vector3 _originalPosition;

        private Quaternion _originalRotation;

        public void Initialize()
        {
            if (IsInitalized == true) return;

            _originalPosition = this.transform.position;
            _originalRotation = this.transform.rotation;

            OnInitialize();

            IsInitalized = true;
        }

        public void DeInitialize()
        {
            if (IsInitalized == false) return;

            OnDeinitialize();

            IsInitalized = false;
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnDeinitialize() { }

        public void ResetTransform()
        {
            this.transform.SetPositionAndRotation(_originalPosition, _originalRotation);
        }
    }
}
