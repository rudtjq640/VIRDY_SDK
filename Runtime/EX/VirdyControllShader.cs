using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIRDY;
using Fusion;
using UnityEngine.SceneManagement;

namespace VIRDY.SDK
{
    public class VirdyControlShader : MonoBehaviour
    {
        public List<NetworkObject> NetworkObjects = new List<NetworkObject>();

        [SerializeField]
        private List<Material> materials = new List<Material>();
        private Scene currentScene;
        private GameObject[] rootObjects;
        private void OnEnable()
        {
            currentScene = gameObject.scene;

            // 해당 씬의 루트 오브젝트들 가져오기
            rootObjects = currentScene.GetRootGameObjects();

            foreach (GameObject rootObject in rootObjects)
            {
                NetworkObject networkObject = rootObject.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    NetworkObjects.Add(networkObject);
                }
            }
        }

        private void Update()
        {
            SetGameObject();
        }
        public void SetGameObject()
        {
            GameObject[] newRootObjects = currentScene.GetRootGameObjects();
            if (newRootObjects.Length != rootObjects.Length)
            {
                rootObjects = newRootObjects;
                foreach (GameObject rootObject in rootObjects)
                {
                    NetworkObject networkObject = rootObject.GetComponent<NetworkObject>();
                    if (networkObject != null)
                    {
                        if (!NetworkObjects.Contains(networkObject))
                        {
                            NetworkObjects.Add(networkObject);
                        }
                    }
                }
            }
        }

        private void GetMaterials(NetworkObject networkObject)
        {
            var renderers = networkObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    if (!materials.Contains(material))
                    {
                        materials.Add(material);
                    }
                }
            }
        }

        public void SetShaderColor()
        {
            materials.Clear();
            foreach (NetworkObject networkObject in NetworkObjects)
            {
                GetMaterials(networkObject);
            }
            foreach (Material material in materials)
            {
                material.SetColor("_BaseColor", new Color(1, 0, 0, 1));
            }
        }
    }
}
