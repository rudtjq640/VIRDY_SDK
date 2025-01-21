using UnityEngine;
using TMPro;

namespace VIRDY.SDK
{
    public class VIRDYMultiObjectControl : MonoBehaviour
    {
        public GameObject[] Objects;
        private int _currentObject = 0;

        public TextMeshProUGUI ObjectIndexText;

        public void TurnOnNextObject()
        {
            if (_currentObject < Objects.Length)
            {
                Objects[_currentObject].SetActive(true);
                _currentObject++;
                UpdateText();
            }
        }

        public void TurnOffPreviousObject()
        {
            if (_currentObject > 0)
            {
                _currentObject--;
                Objects[_currentObject].SetActive(false);
                UpdateText();
            }
        }

        private void UpdateText()
        {
            if (_currentObject == 0)
            {
                ObjectIndexText.text = "0";
            }
            else
            {
                ObjectIndexText.text = _currentObject.ToString();
            }
        }
    }
}
