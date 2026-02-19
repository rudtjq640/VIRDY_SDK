using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRM;

public class BlendShapeManager : MonoBehaviour
{
    [System.Serializable]
    public class BlendShapeTarget
    {
        public int TargetIndex;
        [Header("VRM Meta Title")]
        public string VRMMetaTitle;

        [Header("BlendShapeTarget Path - Root 제외")]
        public string BlendShapeTargetPath;
        [Header("VRM BlendShape Target Name")]
        public string BlendShapeTargetName;
        public bool IsOn = true;

        [Header("GameObject Path - Root 제외")]
        public string GameObjectPath;
        public bool ControlGameObject = false;
    }

    public List<BlendShapeTarget> targets = new List<BlendShapeTarget>();

    public void SetBlendShape(int targetIndex)
    {
        var t = targets.Find(x => x.TargetIndex == targetIndex);
        if (t != null)
        {
            var blendShapeObj = FindObjectByPath(t.BlendShapeTargetPath, t.VRMMetaTitle);
            if (blendShapeObj != null)
            {
                if (t.ControlGameObject)
                {
                    var gameObjectObj = FindObjectByPath(t.GameObjectPath, t.VRMMetaTitle);
                    if (gameObjectObj != null)
                    {
                        gameObjectObj.SetActive(t.IsOn);
                    }
                }

                var blendShape = blendShapeObj.GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBlendShapeIndex(t.BlendShapeTargetName);
                if (blendShape != -1)
                {
                    blendShapeObj.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(blendShape, t.IsOn ? 100f : 0f);
                }
            }
        }
    }

    private GameObject FindObjectByPath(string path, string VRMMetaTitle)
    {
        string[] pathParts = path.Split('/');
        if (pathParts.Length == 0)
        {
            return null;
        }

        List<GameObject> VRMMetaList = GameObject.FindObjectsOfType<VRMMeta>()
            .Select(vrmMeta => vrmMeta.gameObject)
            .ToList();
        //GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        GameObject current = null;

        foreach (var rootObject in VRMMetaList)
        {
            if (rootObject.GetComponent<VRMMeta>().Meta.Title.Contains(VRMMetaTitle))
            {
                current = SearchBlendShapeTarget(pathParts, rootObject);
                break;
            }
            /*else
            {
                foreach (var child in rootObject.GetComponentsInChildren<Transform>())
                {
                    if (child.name.Contains(pathParts[0]))
                    {
                        current = SearchBlendShapeTarget(pathParts, child.gameObject);
                        break;
                    }
                }
            }*/
        }
        return current;
    }

    private GameObject SearchBlendShapeTarget(string[] pathParts, GameObject current)
    {
        if (current != null)
        {
            for (int i = 0; i < pathParts.Length; i++)
            {
                Transform childTransform = current.transform.Find(pathParts[i]);
                if (childTransform == null)
                {
                    return null;
                }
                current = childTransform.gameObject;
            }
        }
        return current;
    }
}
