using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if VIRDY_DOTWEEN
using DG.Tweening;
#endif

public class JingCar : MonoBehaviour
{
    private float _duration = 0.5f;

    private Transform _frontDoor;

    private Transform _backDoor;

    public void Open()
    {
        if (FindDoor() == false) return;

#if VIRDY_DOTWEEN
        DOTween.Kill(_frontDoor);
        DOTween.Kill(_backDoor);

        _frontDoor.DOLocalRotate(Vector3.zero, _duration);
        _backDoor.DOLocalRotate(Vector3.zero, _duration);
#endif
    }

    public void Close()
    {
        if (FindDoor() == false) return;

#if VIRDY_DOTWEEN
        DOTween.Kill(_frontDoor);
        DOTween.Kill(_backDoor);

        _frontDoor.DOLocalRotate(new Vector3(-90f, 0f, 0f), _duration);
        _backDoor.DOLocalRotate(new Vector3(-90f, 0f, 0f), _duration);
#endif
    }

    private bool FindDoor()
    {
        if (_frontDoor != null && _backDoor != null) return true;

        foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var transform in rootGameObject.GetComponentsInChildren<Transform>())
            {
                if (transform.name == "Car_Front(Rotation)") _frontDoor = transform;
                if (transform.name == "Car_Back(Rotation)") _backDoor = transform;
            }
        }

        return _frontDoor != null && _backDoor != null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(JingCar))]
public class JingCarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var jingCar = (JingCar)target;

        if (Application.isPlaying == true)
        {
            if (GUILayout.Button("Open")) jingCar.Open();
            if (GUILayout.Button("Close")) jingCar.Close();
        }
    }
}
#endif
