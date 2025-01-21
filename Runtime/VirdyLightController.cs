using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIRDYLightController : MonoBehaviour
{
    [System.Serializable]
    public struct Preset
    {
        public Color firstLightColor;
        public Color secondLightColor;
        public Color thirdLightColor;
        public float intensity;
        public float range;
    }

    public Light firstPointLight;
    public Light secondPointLight;
    public Light thirdPointLight;

    public Preset[] presets;

    public float transitionSpeed = 1f;

    public List<ParticleSystem> particleSystems;
    public bool particleEffectEnabled = true;
    public float particleEmissionRate = 10f;

    private IEnumerator LerpLightProperties(Light light, Color targetColor, float targetIntensity, float targetRange)
    {
        float elapsedTime = 0f;
        Color initialColor = light.color;
        float initialIntensity = light.intensity;
        float initialRange = light.range;

        while (elapsedTime < transitionSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionSpeed;

            light.color = Color.Lerp(initialColor, targetColor, t);
            light.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);
            light.range = Mathf.Lerp(initialRange, targetRange, t);

            yield return null;
        }
    }

    public void ApplyPreset(int presetIndex)
    {
        if (presetIndex < 0 || presetIndex >= presets.Length) return;

        StartCoroutine(LerpLightProperties(firstPointLight, presets[presetIndex].firstLightColor, presets[presetIndex].intensity, presets[presetIndex].range));
        StartCoroutine(LerpLightProperties(secondPointLight, presets[presetIndex].secondLightColor, presets[presetIndex].intensity, presets[presetIndex].range));
        StartCoroutine(LerpLightProperties(thirdPointLight, presets[presetIndex].thirdLightColor, presets[presetIndex].intensity, presets[presetIndex].range));

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if (particleSystem)
            {
                var emission = particleSystem.emission;
                emission.enabled = particleEffectEnabled;
            }
        }
    }

    public void ToggleParticleEffect()
    {
        particleEffectEnabled = !particleEffectEnabled;

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if (particleSystem)
            {
                var emission = particleSystem.emission;
                emission.enabled = particleEffectEnabled;
            }
        }
    }
}
