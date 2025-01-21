using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(TMP_InputField))]
    public class VIRDYInputField : VIRDYBehaviour
    {
        public static UnityAction<string> OnInputFieldSelect;

        public static UnityAction<string> OnInputFieldEndEdit;

        private TMP_InputField _inputField;

        protected override void OnInitialize()
        {
            _inputField = this.GetComponent<TMP_InputField>();

            if (_inputField != null)
            {
                _inputField.onSelect.AddListener((str) => OnInputFieldSelect?.Invoke(str));
                _inputField.onEndEdit.AddListener((str) => OnInputFieldEndEdit?.Invoke(str));

                _inputField.onSubmit.AddListener((str) => EventSystem.current.SetSelectedGameObject(null));
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
    }
}
