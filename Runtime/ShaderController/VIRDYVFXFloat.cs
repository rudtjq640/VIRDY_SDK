using UnityEngine;
using UnityEngine.VFX;
using TMPro;

public class VIRDYVFXFloat : MonoBehaviour
{
    [SerializeField]
    private string _propertyName;

    [Space]

    [SerializeField]
    private TMP_InputField _inputField;

    private VisualEffect _visualEffect;

    private float _resetValue;

    public TMP_InputField.OnChangeEvent Initialize(VisualEffect visualEffect)
    {
        _visualEffect = visualEffect;

        float value = _visualEffect.GetFloat(_propertyName);
        _inputField.SetTextWithoutNotify(value.ToString());
        _resetValue = value;

        _inputField.onValueChanged.AddListener(OnValueChanged);

        return _inputField.onValueChanged;
    }

    private void Update()
    {
        if (_visualEffect == null || _inputField.isFocused)
            return;

        _inputField.SetTextWithoutNotify(_visualEffect.GetFloat(_propertyName).ToString());
    }

    private void OnValueChanged(string text)
    {
        if (float.TryParse(text, out float value))
        {
            _visualEffect.SetFloat(_propertyName, value);
        }
    }

    public void SetValue(float value)
    {
        _visualEffect.SetFloat(_propertyName, value);
    }

    public void ResetValue()
    {
        _visualEffect.SetFloat(_propertyName, _resetValue);
    }
}
