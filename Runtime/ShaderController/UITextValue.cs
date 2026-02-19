using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITextValue : MonoBehaviour
{
    public TextMeshProUGUI percentageText;
    public Slider sliderUI;

    void Start()
    {

    }

    public void Update()
    {
        percentageText.text = sliderUI.value.ToString("0.00");
    }
}
