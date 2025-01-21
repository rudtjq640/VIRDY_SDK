using UnityEngine;
using System.Collections.Generic;

namespace VIRDY.SDK
{
    public class VIRDYObjectActivator : MonoBehaviour
    {
        public List<string> targetObjectNames = new List<string>();  // 찾고자 하는 오브젝트 이름 목록

        // 모든 대상 오브젝트를 활성화
        public void ActivateObjects()
        {
            foreach (GameObject obj in FindTargetObjects())
            {
                obj.SetActive(true);
            }
        }

        // 모든 대상 오브젝트를 비활성화
        public void DeactivateObjects()
        {
            foreach (GameObject obj in FindTargetObjects())
            {
                obj.SetActive(false);
            }
        }

        // 씬 내에서 특정 이름을 가진 오브젝트 찾기
        private List<GameObject> FindTargetObjects()
        {
            List<GameObject> targetObjects = new List<GameObject>();
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // 비활성화된 오브젝트를 포함하여 모든 GameObject 찾기
            foreach (GameObject obj in allObjects)
            {
                // 오브젝트의 이름이 targetObjectNames 리스트에 있는지 확인
                foreach (string targetName in targetObjectNames)
                {
                    if (obj.name.Contains(targetName))
                    {
                        targetObjects.Add(obj);
                        break; // 이미 찾은 오브젝트이므로 추가 검색 중지
                    }
                }
            }
            return targetObjects;
        }
    }
}
