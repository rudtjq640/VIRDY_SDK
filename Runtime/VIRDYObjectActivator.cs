using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace VIRDY.SDK
{
    public class VIRDYObjectActivator : MonoBehaviour
    {
        public List<string> targetObjectNames = new List<string>();

        // 해당 씬에서만 동작하도록 Awake 시점에 씬 정보 저장
        private Scene _owningScene;

        private void Awake()
        {
            _owningScene = gameObject.scene;
        }

        // 모든 대상 오브젝트를 활성화
        public void ActivateObjects()
        {
            foreach (var obj in FindTargetObjects())
                obj.SetActive(true);
        }

        // 모든 대상 오브젝트를 비활성화
        public void DeactivateObjects()
        {
            foreach (var obj in FindTargetObjects())
                obj.SetActive(false);
        }

        // 이 컴포넌트가 속한 씬에서만, 이름 필터에 걸리는 오브젝트 찾기
        private List<GameObject> FindTargetObjects()
        {
            var targetObjects = new List<GameObject>();
            // 비활성화된 오브젝트까지 포함해서 씬 전체를 뒤지기 위해 모든 GameObject 가져오기
            var allObjects = FindObjectsOfType<GameObject>(true);

            foreach (var obj in allObjects)
            {
                // 씬이 다르면 건너뛰기
                if (obj.scene != _owningScene)
                    continue;

                // 이름 키워드 검사
                foreach (var key in targetObjectNames)
                {
                    if (!string.IsNullOrEmpty(key) && obj.name.Contains(key))
                    {
                        targetObjects.Add(obj);
                        break;
                    }
                }
            }

            return targetObjects;
        }
    }
}
