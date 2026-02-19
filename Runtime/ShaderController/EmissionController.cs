using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public string targetMaterialName = "CharacterMaterial";  // 조절하고자 하는 머티리얼의 이름
    public float emissionIntensity = 0.5f;                   // 조절하고자 하는 Emission Intensity 값

    void Update()
    {
        UpdateEmissionIntensity();
    }

    private void UpdateEmissionIntensity()
    {
        Renderer[] allRenderers = FindObjectsOfType<Renderer>(); // 모든 Renderer 컴포넌트 검색
        foreach (Renderer renderer in allRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.name.Contains(targetMaterialName))  // 해당 머티리얼 이름을 포함하고 있는지 확인
                {
                    // _EmissionIntensity 프로퍼티를 조절
                    material.SetFloat("_EmissionIntensity", emissionIntensity);
                    // Emission 색상 활성화 필요 시 아래 코드 활성화
                    // Color currentColor = material.GetColor("_EmissionColor");
                    // material.SetColor("_EmissionColor", currentColor * Mathf.LinearToGammaSpace(emissionIntensity));
                }
            }
        }
    }
}
