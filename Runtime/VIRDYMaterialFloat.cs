using TMPro;
using UnityEngine;

namespace VIRDY.SDK
{
    public class VIRDYMaterialFloat : VIRDYBehaviour
    {
        [SerializeField]
        private string _propertyName;

        [Space]

        [SerializeField]
        private TMP_InputField _inputField;

        private Material _material;

        private float _resetValue;

        public TMP_InputField.OnChangeEvent Initialize(Material material)
        {
            _material = material;

            if (!_material.HasFloat(_propertyName))
            {
                this.enabled = false;
                Debug.LogWarning("material does not have property: " + _propertyName);
                return null;
            }

            float value = _material.GetFloat(_propertyName);
            _inputField.SetTextWithoutNotify(value.ToString());
            _resetValue = value;

            _inputField.onValueChanged.AddListener(OnValueChanged);

            return _inputField.onValueChanged;
        }

        private void Update()
        {
            if (_material == null || _inputField.isFocused)
                return;

            _inputField.SetTextWithoutNotify(_material.GetFloat(_propertyName).ToString());
        }

        private void OnValueChanged(string text)
        {
            if (float.TryParse(text, out float value))
            {
                _material.SetFloat(_propertyName, value);
            }
        }

        public void SetValue(float value)
        {
            _material.SetFloat(_propertyName, value);
        }

        public void ResetValue()
        {
            _material.SetFloat(_propertyName, _resetValue);
        }
    }
}
