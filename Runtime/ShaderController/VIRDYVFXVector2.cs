using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Events;
using TMPro;

public class VIRDYVFXVector2 : MonoBehaviour
{
    [SerializeField]
    private string _propertyName;

    [Space]

    [SerializeField]
    private TMP_InputField _inputFieldX;
    [SerializeField]
    private TMP_InputField _inputFieldY;

    private VisualEffect _visualEffect;

    public Vector2Event onValueChanged { get; set; }
    public class Vector2Event : UnityEvent<Vector2> { }

    private Vector2 _resetValue;

    public UnityEvent<Vector2> Initialize(VisualEffect visualEffect)
    {
        _visualEffect = visualEffect;

        onValueChanged = new Vector2Event();

        Vector2 vector2 = _visualEffect.GetVector2(_propertyName);
        _inputFieldX.SetTextWithoutNotify(vector2.x.ToString());
        _inputFieldY.SetTextWithoutNotify(vector2.y.ToString());
        _resetValue = vector2;

        _inputFieldX.onValueChanged.AddListener(OnXValueChanged);
        _inputFieldY.onValueChanged.AddListener(OnYValueChanged);

        return onValueChanged;
    }

    private void Update()
    {
        if (_visualEffect == null || _inputFieldX.isFocused || _inputFieldY.isFocused)
            return;

        Vector2 vector2 = _visualEffect.GetVector2(_propertyName);
        _inputFieldX.SetTextWithoutNotify(vector2.x.ToString());
        _inputFieldY.SetTextWithoutNotify(vector2.y.ToString());
    }

    private void OnXValueChanged(string text)
    {
        if (float.TryParse(text, out float x) && float.TryParse(_inputFieldY.text, out float y))
        {
            _visualEffect.SetVector2(_propertyName, new Vector2(x, y));

            onValueChanged.Invoke(_visualEffect.GetVector2(_propertyName));
        }
    }

    private void OnYValueChanged(string text)
    {
        if (float.TryParse(_inputFieldX.text, out float x) && float.TryParse(text, out float y))
        {
            _visualEffect.SetVector2(_propertyName, new Vector2(x, y));

            onValueChanged.Invoke(_visualEffect.GetVector2(_propertyName));
        }
    }

    public void SetValue(Vector2 value)
    {
        _visualEffect.SetVector2(_propertyName, value);
    }

    public void ResetValue()
    {
        _visualEffect.SetVector2(_propertyName, _resetValue);
    }
}
