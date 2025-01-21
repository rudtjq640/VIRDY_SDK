using UnityEngine;
using UnityEngine.UI;
#if VIRDY_UWC
using uWindowCapture;
#endif

namespace VIRDY.SDK
{
    [RequireComponent(typeof(RawImage))]
    public class VIRDYUwcWindowImage : VIRDYBehaviour
    {
#if VIRDY_UWC
        public UwcWindowTexture WindowTexture;

        private RawImage _rawImage;

        private void Awake()
        {
            if (WindowTexture == null)
            {
                this.enabled = false;
                return;
            }

            _rawImage = this.GetComponent<RawImage>();
        }

        private void Update()
        {
            if (!WindowTexture.isValid)
            {
                _rawImage.texture = null;
                return;
            }

            if (_rawImage.texture != WindowTexture.window.texture)
            {
                _rawImage.texture = WindowTexture.window.texture;
            }
        }
#endif
    }
}
