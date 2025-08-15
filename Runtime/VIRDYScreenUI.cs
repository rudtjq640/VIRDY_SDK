using UnityEngine;
using UnityEngine.Events;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(Canvas))]
    public class VIRDYScreenUI : VIRDYBehaviour
    {
        public static GetParentDelegate GetParent;
        public delegate RectTransform GetParentDelegate();

        public static GetCameraDelegate GetCamera;
        public delegate Camera GetCameraDelegate();

        public static UnityAction FirstSibling, LastSibling;

        private static Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        protected override void OnInitialize()
        {
            gameObject.layer = 6;

            var rectTransform = this.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                //if (GetParent != null)
                //{
                //    var parent = GetParent();

                //    if (parent != null)
                //    {
                //        rectTransform.SetParent(parent);
                //        rectTransform.anchorMin = new Vector2(0f, 0f);
                //        rectTransform.anchorMax = new Vector2(1f, 1f);
                //        rectTransform.offsetMin = Vector2.zero;
                //        rectTransform.offsetMax = Vector2.zero;
                //        rectTransform.anchoredPosition = Vector3.zero;
                //        rectTransform.localRotation = Quaternion.identity;
                //        rectTransform.localScale = Vector3.one;
                //    }
                //}

                //_canvas = this.GetComponent<Canvas>();
                SetCanvasRenderMode();

                FirstSibling += SetAsFirstSibling;
                LastSibling += SetAsLastSibling;
            }
        }
        protected override void OnDeinitialize()
        {
            FirstSibling -= SetAsFirstSibling;
            LastSibling -= SetAsLastSibling;
        }

        private void SetAsFirstSibling()
        {
            this.GetComponent<RectTransform>()?.SetAsFirstSibling();
        }
        private void SetAsLastSibling()
        {
            this.GetComponent<RectTransform>()?.SetAsLastSibling();
        }

        /// <summary>
        /// ScreenUI Canvas의 RenderMode를 변경.
        /// </summary>
        public static void SetCanvasRenderMode()
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = GetCamera();
            _canvas.planeDistance = 0.15f;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (_canvas == null)
            {
                _canvas = this.GetComponent<Canvas>();
            }
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
#endif
    }
}
