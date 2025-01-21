using UnityEngine;

namespace VIRDY.SDK
{
    public class VIRDYDissolveController : MonoBehaviour
    {
        [Range(0f, 1f)] public float dissolveAmount;
        [Range(0f, 3f)] public float radius;

        public float duration = 1f; // 변경하는 데 걸리는 시간

        private float targetDissolveAmount; // 목표 Dissolve Amount 값
        private float targetRadius; // 목표 Radius 값

        private float dissolveVelocity; // Dissolve Amount 변화 속도
        private float radiusVelocity; // Radius 변화 속도

        void OnValidate()
        {
            ApplyChanges();
        }

        void Update()
        {
            // dissolveAmount, Radius 값을 천천히 변경
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
            dissolveVelocity = 0f; // 변경 속도 초기화
        }

        public void DissolveOFF()
        {
            targetDissolveAmount = 0f;
            dissolveVelocity = 0f; // 변경 속도 초기화
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