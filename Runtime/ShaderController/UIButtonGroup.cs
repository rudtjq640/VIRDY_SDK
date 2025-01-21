using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIButtonGroup : MonoBehaviour
{
    public List<Button> buttons = new List<Button>(); // 버튼 목록

    public Color selectedColor = Color.green; // 선택된 버튼의 색
    public Color normalColor = Color.white; // 일반 버튼의 색

    private Button selectedButton; // 현재 선택된 버튼

    void Start()
    {
        // 모든 버튼에 대해 이벤트 리스너 설정
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => SetActiveButton(button));
        }

        // 초기 버튼 설정 (옵션, 기본적으로 첫 번째 버튼 선택)
        if (buttons.Count > 0)
        {
            SetActiveButton(buttons[0]);
        }
    }

    void SetActiveButton(Button button)
    {
        // 모든 버튼을 일반 색상으로 설정
        foreach (Button btn in buttons)
        {
            btn.image.color = normalColor;
        }

        // 새로 선택된 버튼을 선택된 색상으로 설정
        selectedButton = button;
        selectedButton.image.color = selectedColor;
    }
}
