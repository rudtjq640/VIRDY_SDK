using UnityEngine;
using UnityEngine.Events;

namespace VIRDY.SDK
{
    public class VIRDYCameraUI : VIRDYBehaviour
    {
        public static UnityAction<Camera> OnRenderCameraChange;

        public static GetEventCameraDelegate GetRenderCamera;

        public delegate Camera GetEventCameraDelegate();

        private Canvas _canvas;

        protected override void OnInitialize()
        {
            _canvas = this.GetComponent<Canvas>();

            if (_canvas != null)
            {
                if (GetRenderCamera != null)
                {
                    _canvas.worldCamera = GetRenderCamera();
                    OnRenderCameraChange += OnRenderCameraChanged;
                }
            }
        }

        protected override void OnDeinitialize()
        {
            OnRenderCameraChange -= OnRenderCameraChanged;
        }

        private void OnRenderCameraChanged(Camera camera)
        {
            _canvas.worldCamera = camera;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            var canvas = this.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
#endif
    }
}
