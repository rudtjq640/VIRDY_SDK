using UnityEngine;
using UnityEngine.UI;
#if VIRDY_UWC
using uWindowCapture;
#endif

namespace VIRDY.SDK
{
    public class VIRDYUwcWindowListItem : VIRDYBehaviour
    {
#if VIRDY_UWC
        public UwcWindow Window { get; set; }
        public VIRDYUwcWindowList List { get; set; }

        private UwcWindowTexture _windowTexture;

        Image _image;
        [SerializeField] Color selected;
        [SerializeField] Color notSelected;

        [SerializeField] RawImage icon;
        [SerializeField] Text title;
        [SerializeField] Text x;
        [SerializeField] Text y;
        [SerializeField] Text z;
        [SerializeField] Text width;
        [SerializeField] Text height;
        [SerializeField] Text status;

        void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = notSelected;
        }

        void Update()
        {
            if (Window == null) return;

            if (!Window.hasIconTexture && !Window.isIconic)
            {
                icon.texture = Window.texture;
            }
            else
            {
                icon.texture = Window.iconTexture;
            }

            var windowTitle = Window.title;
            title.text = string.IsNullOrEmpty(windowTitle) ? "-No Name-" : windowTitle;

            x.text = Window.isMinimized ? "-" : Window.x.ToString();
            y.text = Window.isMinimized ? "-" : Window.y.ToString();
            z.text = Window.zOrder.ToString();

            width.text = Window.width.ToString();
            height.text = Window.height.ToString();

            status.text =
                Window.isIconic ? "Iconic" :
                Window.isZoomed ? "Zoomed" :
                "-";
        }

        public void OnClick()
        {
            if (_windowTexture == null)
            {
                AddWindow();
            }
            else
            {
                RemoveWindow();
            }
        }

        void AddWindow()
        {
            List.RemoveWindow();

            var windowTexture = List.WindowTexture;
            windowTexture.window = Window;
            _windowTexture = windowTexture;
            _image.color = selected;
        }

        public void RemoveWindow()
        {
            var windowTexture = List.WindowTexture;
            windowTexture.window = null;
            _windowTexture = null;
            _image.color = notSelected;
        }
#endif
    }
}
