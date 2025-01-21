using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class VIRDYVFXSlider : MonoBehaviour
{
    [SerializeField]
    private string _propertyName;

    [Space]

    [SerializeField]
    private Slider _slider;

    private VisualEffect _visualEffect;

    private float _resetValue;

    public Slider.SliderEvent Initialize(VisualEffect visualEffect)
    {
        _visualEffect = visualEffect;

        float value = _visualEffect.GetFloat(_propertyName);
        _slider.SetValueWithoutNotify(value);
        _resetValue = value;

        _slider.onValueChanged.AddListener(OnValueChanged);

        return _slider.onValueChanged;
    }

    private void Update()
    {
        if (_visualEffect == null)
            return;

        _slider.SetValueWithoutNotify(_visualEffect.GetFloat(_propertyName));
    }

    private void OnValueChanged(float value)
    {
        _visualEffect.SetFloat(_propertyName, _slider.value);
    }

    public void SetValue(float value)
    {
        _slider.value = value;
    }

    public void ResetValue()
    {
        _slider.value = _resetValue;
    }
}
