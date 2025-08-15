using UnityEngine;
using UnityEngine.SceneManagement;

public class VIRDYManualPickup : MonoBehaviour
{
    [Header("Avatar & Hand Search")]
    [SerializeField] private string _characterName = "Gosegu";
    [SerializeField] private string _handName = "LeftHand";

    [Header("Local Transform")]
    [SerializeField] private Vector3 _localPosition;
    [SerializeField] private Quaternion _localRotation;

    [Header("Reset Transform")]
    [SerializeField] private Vector3 _resetPosition;
    [SerializeField] private Quaternion _resetRotation;

    public void ManualPickup()
    {
        Transform characterTransform = FindTransformInMyScene(_characterName);
        if (characterTransform == null)
        {
            Debug.LogWarning($"[ManualPickup] Character not found: {_characterName}");
            return;
        }

        Transform handTransform = FindChildRecursive(characterTransform, _handName);
        if (handTransform == null)
        {
            Debug.LogWarning($"[ManualPickup] Hand not found under {_characterName}: {_handName}");
            return;
        }

        this.transform.SetParent(handTransform);
        this.transform.localPosition = _localPosition;
        this.transform.localRotation = _localRotation;

        Debug.Log($"[ManualPickup] '{this.gameObject.name}' is now parented to '{handTransform.name}'.");
    }

    public void ManualDrop()
    {
        this.transform.SetParent(null);
        this.transform.SetPositionAndRotation(_resetPosition, _resetRotation);
    }

    [ContextMenu("Set Local Transform")]
    public void SetLocalTransform()
    {
        _localPosition = this.transform.localPosition;
        _localRotation = this.transform.localRotation;
    }

    [ContextMenu("Set Reset Transform")]
    public void SetResetTransform()
    {
        _resetPosition = this.transform.position;
        _resetRotation = this.transform.rotation;
    }

    private Transform FindTransformInMyScene(string targetName)
    {
        Scene myScene = this.gameObject.scene;

        foreach (GameObject rootObj in myScene.GetRootGameObjects())
        {
            Transform found = FindChildRecursive(rootObj.transform, targetName);
            if (found != null)
                return found;
        }

        return null;
    }

    private Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent.name == childName)
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindChildRecursive(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }
}
