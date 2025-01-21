using System.Collections.Generic;
using UnityEngine;
#if VIRDY_UWC
using uWindowCapture;
#endif

namespace VIRDY.SDK
{
    public class VIRDYUwcWindowList : VIRDYBehaviour
    {
#if VIRDY_UWC
        public UwcWindowTexture WindowTexture;

        [Space]

        [SerializeField]
        private GameObject _windowListItem;
        [SerializeField]
        private Transform _listRoot;

        private Dictionary<int, VIRDYUwcWindowListItem> _items = new Dictionary<int, VIRDYUwcWindowListItem>();

        private void Start()
        {
            UwcManager.onWindowAdded.AddListener(OnWindowAdded);
            UwcManager.onWindowRemoved.AddListener(OnWindowRemoved);

            foreach (var pair in UwcManager.windows)
            {
                OnWindowAdded(pair.Value);
            }
        }

        private void OnWindowAdded(UwcWindow window)
        {
            if (!window.isAltTabWindow || window.isBackground) return;

            var gameObject = Instantiate(_windowListItem, _listRoot, false);
            var listItem = gameObject.GetComponent<VIRDYUwcWindowListItem>();
            listItem.Window = window;
            listItem.List = this;
            _items.Add(window.id, listItem);

            window.RequestCaptureIcon();
            window.RequestCapture(CapturePriority.Low);
        }

        private void OnWindowRemoved(UwcWindow window)
        {
            VIRDYUwcWindowListItem listItem;
            _items.TryGetValue(window.id, out listItem);
            if (listItem)
            {
                listItem.RemoveWindow();
                Destroy(listItem.gameObject);
            }
        }

        public void RemoveWindow()
        {
            foreach (var item in _items.Values)
                item.RemoveWindow();
        }
#endif
    }
}
