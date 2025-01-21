using UnityEngine;
using UnityEngine.Events;

namespace VIRDY.SDK
{
    public class VIRDYWorldUI : VIRDYBehaviour
    {
        public static UnityAction<Camera> OnEventCameraChange;

        public static GetEventCameraDelegate GetEventCamera;

        public delegate Camera GetEventCameraDelegate();

        private Canvas _canvas;

        protected override void OnInitialize()
        {
            _canvas = this.GetComponent<Canvas>();

            if (_canvas != null)
            {
                if (GetEventCamera != null)
                {
                    _canvas.worldCamera = GetEventCamera();
                    OnEventCameraChange += OnEventCameraChanged;
                }
            }
        }

        protected override void OnDeinitialize()
        {
            OnEventCameraChange -= OnEventCameraChanged;
        }

        private void OnEventCameraChanged(Camera camera)
        {
            _canvas.worldCamera = camera;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            var canvas = this.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
        }
#endif
    }
}
