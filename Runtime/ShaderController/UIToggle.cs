using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Toggle))]
public class UIToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Sprite onSprite;
    public Sprite offSprite;
    public Color onColor;
    public Color offColor;
    public Color highlightedColor;
    public Color pressedColor;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void Start()
    {
        // Initial state
        OnToggleValueChanged(toggle.isOn);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            SetActiveState();
        }
        else
        {
            toggle.image.color = offColor;
            SetInactiveState();
        }
    }

    public void SetActiveState()
    {
        toggle.image.sprite = onSprite;
        toggle.image.color = onColor;

        toggle.SetIsOnWithoutNotify(true);
    }

    public void SetInactiveState()
    {
        toggle.image.sprite = offSprite;

        toggle.SetIsOnWithoutNotify(false);
    }

    // Pointer event handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (toggle.interactable && !toggle.isOn)
            toggle.image.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toggle.interactable && !toggle.isOn)
            toggle.image.color = offColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(toggle.interactable)
            toggle.image.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (toggle.interactable)
        {
            if (toggle.isOn)
                SetActiveState();
            else
                SetInactiveState();
        }
    }
}
