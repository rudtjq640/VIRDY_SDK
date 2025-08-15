using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIRDY;
using Fusion;
using UnityEngine.SceneManagement;

namespace VIRDY.SDK
{
    public class VirdyControlGameobject : MonoBehaviour
    {
        [SerializeField]
        private GameObject GameObject;
        [HideInInspector]
        public List<NetworkObject> NetworkObjects = new List<NetworkObject>();

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

            foreach (NetworkObject networkObject in NetworkObjects)
            {
                networkObject.transform.SetParent(GameObject.transform);
                //networkObject.transform.position = new Vector3(networkObject.transform.position.x, GameObject.transform.position.y, networkObject.transform.position.z);
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
                        NetworkObjects.Add(networkObject);
                    }
                }

                foreach (NetworkObject networkObject in NetworkObjects)
                {
                    networkObject.transform.SetParent(GameObject.transform);
                    //networkObject.transform.position = new Vector3(networkObject.transform.position.x, GameObject.transform.position.y, networkObject.transform.position.z);
                }
            }
        }
    }
}
