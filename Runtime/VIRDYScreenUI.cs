using UnityEngine;
using UnityEngine.Events;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(Canvas))]
    public class VIRDYScreenUI : VIRDYBehaviour
    {
        public static GetParentDelegate GetParent;

        public delegate RectTransform GetParentDelegate();

        public static UnityAction FirstSibling, LastSibling;

        protected override void OnInitialize()
        {
            var rectTransform = this.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                if (GetParent != null)
                {
                    var parent = GetParent();

                    if (parent != null)
                    {
                        rectTransform.SetParent(parent);
                        rectTransform.anchorMin = new Vector2(0f, 0f);
                        rectTransform.anchorMax = new Vector2(1f, 1f);
                        rectTransform.offsetMin = Vector2.zero;
                        rectTransform.offsetMax = Vector2.zero;
                        rectTransform.anchoredPosition = Vector3.zero;
                        rectTransform.localRotation = Quaternion.identity;
                        rectTransform.localScale = Vector3.one;
                    }
                }

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

#if UNITY_EDITOR
        private void Reset()
        {
            var canvas = this.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
#endif
    }
}
