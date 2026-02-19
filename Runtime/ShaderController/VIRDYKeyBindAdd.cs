using UnityEngine;

public class VIRDYKeyBindAdd : MonoBehaviour
{
    [Header("Key Bindings")]
    public KeyCode KeyOne = KeyCode.None;
    public KeyCode KeyTwo = KeyCode.None;
    public KeyCode KeyThree = KeyCode.None;

    [Header("Target Object")]
    public GameObject TargetObject;

    [Header("Settings")]
    public float Delay = 0.0f;

    private bool _isPressed = false;
    private float _lastPressTime = 0.0f;

    void Update()
    {
        if (IsCorrectKeyCombinationPressed())
        {
            if (Time.time - _lastPressTime > Delay)
            {
                ToggleObject();
                _lastPressTime = Time.time;
            }
        }
    }

    private void ToggleObject()
    {
        if (TargetObject != null)
        {
            TargetObject.SetActive(!TargetObject.activeSelf);
        }
        else
        {
            Debug.Log("Target Object is not assigned in KeyBind");
        }
    }

    private bool IsCorrectKeyCombinationPressed()
    {
        bool keyOnePressed = KeyOne == KeyCode.None || Input.GetKey(KeyOne);
        bool keyTwoPressed = KeyTwo == KeyCode.None || Input.GetKey(KeyTwo);
        bool keyThreePressed = KeyThree == KeyCode.None || Input.GetKey(KeyThree);

        return keyOnePressed && keyTwoPressed && keyThreePressed;
    }
}
