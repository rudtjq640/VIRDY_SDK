using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public string targetMaterialName = "CharacterMaterial";  // �����ϰ��� �ϴ� ��Ƽ������ �̸�
    public float emissionIntensity = 0.5f;                   // �����ϰ��� �ϴ� Emission Intensity ��

    void Update()
    {
        UpdateEmissionIntensity();
    }

    private void UpdateEmissionIntensity()
    {
        Renderer[] allRenderers = FindObjectsOfType<Renderer>(); // ��� Renderer ������Ʈ �˻�
        foreach (Renderer renderer in allRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.name.Contains(targetMaterialName))  // �ش� ��Ƽ���� �̸��� �����ϰ� �ִ��� Ȯ��
                {
                    // _EmissionIntensity ������Ƽ�� ����
                    material.SetFloat("_EmissionIntensity", emissionIntensity);
                    // Emission ���� Ȱ��ȭ �ʿ� �� �Ʒ� �ڵ� Ȱ��ȭ
                    // Color currentColor = material.GetColor("_EmissionColor");
                    // material.SetColor("_EmissionColor", currentColor * Mathf.LinearToGammaSpace(emissionIntensity));
                }
            }
        }
    }
}
