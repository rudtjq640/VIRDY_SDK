using UnityEngine;

namespace VIRDY.SDK
{
    public class VIRDYDissolveController : MonoBehaviour
    {
        [Range(0f, 1f)] public float dissolveAmount;
        [Range(0f, 3f)] public float radius;

        public float duration = 1f; // �����ϴ� �� �ɸ��� �ð�

        private float targetDissolveAmount; // ��ǥ Dissolve Amount ��
        private float targetRadius; // ��ǥ Radius ��

        private float dissolveVelocity; // Dissolve Amount ��ȭ �ӵ�
        private float radiusVelocity; // Radius ��ȭ �ӵ�

        void OnValidate()
        {
            ApplyChanges();
        }

        void Update()
        {
            // dissolveAmount, Radius ���� õõ�� ����
            dissolveAmount = Mathf.SmoothDamp(dissolveAmount, targetDissolveAmount, ref dissolveVelocity, duration);
            radius = Mathf.SmoothDamp(radius, targetRadius, ref radiusVelocity, duration);
            ApplyChanges();
        }

        public void ApplyChanges()
        {
            foreach (var material in Resources.FindObjectsOfTypeAll<Material>())
            {
                if (material.HasProperty("_DissolveAmount1"))
                    material.SetFloat("_DissolveAmount1", dissolveAmount);
                
                if (material.HasProperty("_DissolveAmount2"))
                    material.SetFloat("_DissolveAmount2", dissolveAmount);
            
                if (material.HasProperty("_Radius"))
                    material.SetFloat("_Radius", radius);
            }
        }

        public void DissolveON()
        {
            targetDissolveAmount = 1f;
            dissolveVelocity = 0f; // ���� �ӵ� �ʱ�ȭ
        }

        public void DissolveOFF()
        {
            targetDissolveAmount = 0f;
            dissolveVelocity = 0f; // ���� �ӵ� �ʱ�ȭ
        }

        public void RadiusON()
        {
            targetRadius = 3f;
            radiusVelocity = 0f;
        }

        public void RadiusOFF()
        {
            targetRadius = 0f;
            radiusVelocity = 0f;
        }
    }
}