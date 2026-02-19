using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Button onButton;
    public Button offButton;

    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    private Button selectedButton;

    void Start()
    {
        // 초기 버튼 이벤트 설정
        onButton.onClick.AddListener(() => SetActiveButton(onButton));
        offButton.onClick.AddListener(() => SetActiveButton(offButton));

        // 초기 상태 설정
        SetActiveButton(onButton); // 기본적으로 'On' 버튼이 활성화
    }

    void SetActiveButton(Button button)
    {
        // 이전에 선택된 버튼을 일반 색상으로 설정
        if (selectedButton != null)
        {
            selectedButton.image.color = normalColor;
        }

        // 새로운 버튼을 선택된 색상으로 설정
        selectedButton = button;
        selectedButton.image.color = selectedColor;
    }
}
