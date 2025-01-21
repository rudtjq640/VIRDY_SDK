using UnityEngine;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(Canvas))]
    public class VIRDYManagerUI : VIRDYBehaviour
    {
        public static GetParentDelegate GetParent;

        public delegate RectTransform GetParentDelegate();

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
            }
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
