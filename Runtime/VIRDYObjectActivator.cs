using UnityEngine;
using System.Collections.Generic;

namespace VIRDY.SDK
{
    public class VIRDYObjectActivator : MonoBehaviour
    {
        public List<string> targetObjectNames = new List<string>();  // ã���� �ϴ� ������Ʈ �̸� ���

        // ��� ��� ������Ʈ�� Ȱ��ȭ
        public void ActivateObjects()
        {
            foreach (GameObject obj in FindTargetObjects())
            {
                obj.SetActive(true);
            }
        }

        // ��� ��� ������Ʈ�� ��Ȱ��ȭ
        public void DeactivateObjects()
        {
            foreach (GameObject obj in FindTargetObjects())
            {
                obj.SetActive(false);
            }
        }

        // �� ������ Ư�� �̸��� ���� ������Ʈ ã��
        private List<GameObject> FindTargetObjects()
        {
            List<GameObject> targetObjects = new List<GameObject>();
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // ��Ȱ��ȭ�� ������Ʈ�� �����Ͽ� ��� GameObject ã��
            foreach (GameObject obj in allObjects)
            {
                // ������Ʈ�� �̸��� targetObjectNames ����Ʈ�� �ִ��� Ȯ��
                foreach (string targetName in targetObjectNames)
                {
                    if (obj.name.Contains(targetName))
                    {
                        targetObjects.Add(obj);
                        break; // �̹� ã�� ������Ʈ�̹Ƿ� �߰� �˻� ����
                    }
                }
            }
            return targetObjects;
        }
    }
}
