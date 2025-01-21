using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

        public Transform SpawnPoint;

        public UniversalRendererData RendererData;

        public bool OverrideRendererData;

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
