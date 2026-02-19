using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace VIRDY.SDK
{
    public class VIRDYWorldDescriptor : MonoBehaviour
    {
        public static VIRDYWorldDescriptor Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<VIRDYWorldDescriptor>();
                return _instance;
            }
        }

        private static VIRDYWorldDescriptor _instance;

        [SerializeField]
        private Transform _spawnPoint;
        
        /// <summary>
        /// 스폰 포인트 Transform. 
        /// 설정되어 있으면 해당 Transform 사용, 없으면 자기 자신(VIRDYWorldDescriptor) 반환.
        /// </summary>
        public Transform SpawnPoint
        {
            get => _spawnPoint != null ? _spawnPoint : transform;
            set => _spawnPoint = value;
        }

        public UniversalRendererData RendererData;

        public bool OverrideRendererData;

        public Volume GlobalVolume;

        [Space]

        public string UnityVersion;

        public string URPVersion;

        // Directional Light 설정
        [Header("Directional Light Settings")]
        public bool UseCustomDirectionalLight = false;
        public Light CustomDirectionalLight;

        private void Awake()
        {
            if (_instance != null && _instance != this) GameObject.Destroy(this.gameObject);
            _instance = this;

            if (GlobalVolume == null)
            {
                // VIRDYWorldDescriptor가 속한 씬에서 Volume 찾기
                var scene = gameObject.scene;
                
                var rootObjects = scene.GetRootGameObjects();
                
                foreach (var rootObject in rootObjects)
                {
                    Volume volume = rootObject.GetComponent<Volume>();
                    if (volume != null)
                    {
                        GlobalVolume = volume;
                        break;
                    }
                }
            }

#if !VIRDY_CORE
            foreach (var behaviour in FindObjectsOfType<VIRDYBehaviour>(true))
            {
                behaviour.Initialize();
            }
            foreach (var behaviour in FindObjectsOfType<VIRDYNetworkBehaviour>(true))
            {
                behaviour.Initialize();
            }
#endif
        }
    }
}
