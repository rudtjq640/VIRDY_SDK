using UnityEngine;
using UnityEngine.UI;

namespace VIRDY.SDK
{
    public class VIRDYMaterialSlider : VIRDYBehaviour
    {
        [SerializeField]
        private string _propertyName;

        [Space]

        [SerializeField]
        private Slider _slider;

        private Material _material;

        private float _resetValue;

        public Slider.SliderEvent Initialize(Material material)
        {
            _material = material;

            if (!_material.HasFloat(_propertyName))
            {
                this.enabled = false;
                Debug.LogWarning("material does not have property: " + _propertyName);
                return null;
            }

            float value = _material.GetFloat(_propertyName);
            _slider.SetValueWithoutNotify(value);
            _resetValue = value;

            _slider.onValueChanged.AddListener(OnValueChanged);

            return _slider.onValueChanged;
        }

        private void Update()
        {
            if (_material == null)
                return;

            _slider.SetValueWithoutNotify(_material.GetFloat(_propertyName));
        }

        private void OnValueChanged(float value)
        {
            _material.SetFloat(_propertyName, _slider.value);
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
