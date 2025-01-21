using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Fusion;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(TMP_InputField))]
    public class VIRDYInputFieldSync : VIRDYNetworkBehaviour
    {
        public static UnityAction<string> OnInputFieldSelect;

        public static UnityAction<string> OnInputFieldEndEdit;

        [SerializeField]
        private TextMeshProUGUI[] _syncTargets;

        private TMP_InputField _inputField;

        protected override void OnInitialize()
        {
            _inputField = this.GetComponent<TMP_InputField>();

            if (_inputField != null)
            {
                _inputField.onSelect.AddListener((str) => OnInputFieldSelect?.Invoke(str));
                _inputField.onEndEdit.AddListener((str) => OnInputFieldEndEdit?.Invoke(str));

                _inputField.onSubmit.AddListener((str) => EventSystem.current.SetSelectedGameObject(null));
                _inputField.onEndEdit.AddListener(SendText);
            }
        }

        protected override void OnDeinitialize()
        {
            if (_inputField != null)
            {
                _inputField.onSelect.RemoveAllListeners();
                _inputField.onEndEdit.RemoveAllListeners();
            }
        }

        private void SendText(string str)
        {
            RPC_SyncText(str);
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
        private void RPC_SyncText(string str)
        {
            string formattedText = str.Replace("/", "\n");
            VIRDYSubtitleUI.SetSubtitleText?.Invoke(formattedText);
            // foreach (var text in _syncTargets)
            // {
            //     text.text = str;
            // }
        }
    }
}
