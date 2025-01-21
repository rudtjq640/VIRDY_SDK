using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VIRDYSliderValueText : VIRDYBehaviour
    {
        [SerializeField]
        private Slider _slider;

        [Space]
        [SerializeField]
        private string _format = "F2";
        [SerializeField]
        private string _prefix = "";
        [SerializeField]
        private string _suffix = "";

        private TextMeshProUGUI _valueText;

        protected override void OnInitialize()
        {
            _valueText = this.GetComponent<TextMeshProUGUI>();
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            if (_valueText == true)
            {
                _valueText.text = _prefix + value.ToString(_format) + _suffix;
            }
        }
    }
}
