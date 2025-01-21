using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using VLB;  // VolumetricLightBeam 네임스페이스 상속

namespace VIRDY.SDK
{
    [Serializable]
    // 기본 라이트
    public struct LightSettings
    {
        public Light Light;
        public float TargetIntensity;
        public Color TargetColor;
        public float TargetRange;
        public Vector3 TargetPosition;
        public Quaternion TargetRotation;
        public bool UsePosition;
        public bool UseRotation;
    }

    [Serializable]
    // Material
    public struct MaterialSettings
    {
        public Material Material;
        public Color TargetColor;
        public string Property;
    }

    [Serializable]
    // Volumetric Light Beam
    public struct VolumetricLightBeamSettings
    {
        public VolumetricLightBeamHD VolumetricLightBeam;
        public float TargetIntensity;
        public Color TargetColor;
        public float TargetSpotAngle;
        public float TargetRange;
    }

    [Serializable]
    public struct GlobalVolumeSettings
    {
        public Volume Volume;
        public float TargetWeight;
    }

    [Serializable]
    // 기본 프리셋
    public struct Preset
    {
        public LightSettings[] LightSettings;
        public MaterialSettings[] MaterialSettings;
        public VolumetricLightBeamSettings[] VolumetricLightBeamSettings;
        public GlobalVolumeSettings[] GlobalVolumeSettings;
        public float Duration;
        public AnimationCurve Curve;
        public float TransformDuration;
        public AnimationCurve TransformCurve;
    }

    public class VIRDYSkySetChanger : VIRDYBehaviour
    {
        public Preset[] Presets;
        private Coroutine _currentTransition;

        [SerializeField]
        private UnityEvent _onUpdate = new UnityEvent();

        public void SetPreset(int index)
        {
            if (index < 0 || index >= Presets.Length)
            {
                Debug.LogError("Preset index out of range");
                return;
            }

            if (_currentTransition != null)
                StopCoroutine(_currentTransition);

            _currentTransition = StartCoroutine(ApplyPreset(index));
        }

        private IEnumerator ApplyPreset(int index)
        {
            var preset = Presets[index];
            float elapsed = 0f;
            float transformElapsed = 0f;

            List<float> initialIntensities = new List<float>();
            List<Color> initialColors = new List<Color>();
            List<float> initialRanges = new List<float>();
            List<float> initialSpotAngles = new List<float>();
            List<Vector3> initialPositions = new List<Vector3>();
            List<Quaternion> initialRotation = new List<Quaternion>();

            foreach (var lightSettings in preset.LightSettings)
            {
                initialIntensities.Add(lightSettings.Light.intensity);
                initialColors.Add(lightSettings.Light.color);
                initialRanges.Add(lightSettings.Light.range);
                initialPositions.Add(lightSettings.Light.transform.position);
                initialRotation.Add(lightSettings.Light.transform.rotation);
            }

            List<Color> materialInitialColors = new List<Color>();
            foreach (var materialSettings in preset.MaterialSettings)
            {
                materialInitialColors.Add(materialSettings.Material.GetColor(materialSettings.Property));
            }

            List<float> initialBeamIntensities = new List<float>();
            List<Color> initialBeamColors = new List<Color>();
            List<float> initialBeamSpotAngles = new List<float>();
            List<float> initialBeamRanges = new List<float>();

            foreach (var beamSettings in preset.VolumetricLightBeamSettings)
            {
                initialBeamIntensities.Add(beamSettings.VolumetricLightBeam.intensity);
                initialBeamColors.Add(beamSettings.VolumetricLightBeam.colorFlat);
                initialBeamSpotAngles.Add(beamSettings.VolumetricLightBeam.spotAngle);
                initialBeamRanges.Add(beamSettings.VolumetricLightBeam.fallOffEnd);
            }

            List<float> initialVolumeWeight = new List<float>();
            foreach (var volumeSettings in preset.GlobalVolumeSettings)
            {
                initialVolumeWeight.Add(volumeSettings.Volume.weight);
            }

            while (elapsed < preset.Duration || transformElapsed < preset.TransformDuration)
            {
                elapsed += Time.deltaTime;
                transformElapsed += Time.deltaTime;

                // float t = preset.Curve.Evaluate(elapsed / preset.Duration);
                float t = preset.Curve.Evaluate(Mathf.Min(elapsed / preset.Duration, 1.0f));
                float transformT = preset.TransformCurve.Evaluate(Mathf.Min(transformElapsed / preset.TransformDuration, 1.0f));

                for (int i = 0; i < preset.LightSettings.Length; i++)
                {
                    var settings = preset.LightSettings[i];
                    settings.Light.intensity = Mathf.Lerp(initialIntensities[i], settings.TargetIntensity, t);
                    settings.Light.color = Color.Lerp(initialColors[i], settings.TargetColor, t);
                    settings.Light.range = Mathf.Lerp(initialRanges[i], settings.TargetRange, t);

                    if (settings.UsePosition)
                        settings.Light.transform.position = Vector3.Lerp(initialPositions[i], settings.TargetPosition, transformT);
                    if (settings.UseRotation)
                        settings.Light.transform.rotation = Quaternion.Lerp(initialRotation[i], settings.TargetRotation, transformT);
                }

                for (int i = 0; i < preset.MaterialSettings.Length; i++)
                {
                    preset.MaterialSettings[i].Material.SetColor(preset.MaterialSettings[i].Property, Color.Lerp(materialInitialColors[i], preset.MaterialSettings[i].TargetColor, t));
                }

                for (int i = 0; i < preset.VolumetricLightBeamSettings.Length; i++)
                {
                    preset.VolumetricLightBeamSettings[i].VolumetricLightBeam.intensity = Mathf.Lerp(initialBeamIntensities[i], preset.VolumetricLightBeamSettings[i].TargetIntensity, t);
                    preset.VolumetricLightBeamSettings[i].VolumetricLightBeam.colorFlat = Color.Lerp(initialBeamColors[i], preset.VolumetricLightBeamSettings[i].TargetColor, t);
                    preset.VolumetricLightBeamSettings[i].VolumetricLightBeam.spotAngle = Mathf.Lerp(initialBeamSpotAngles[i], preset.VolumetricLightBeamSettings[i].TargetSpotAngle, t);
                    preset.VolumetricLightBeamSettings[i].VolumetricLightBeam.fallOffEnd = Mathf.Lerp(initialBeamRanges[i], preset.VolumetricLightBeamSettings[i].TargetRange, t);
                }

                for (int i = 0; i < preset.GlobalVolumeSettings.Length; i++)
                {
                    preset.GlobalVolumeSettings[i].Volume.weight = Mathf.Lerp(initialVolumeWeight[i], preset.GlobalVolumeSettings[i].TargetWeight, t);
                }

                _onUpdate?.Invoke();

                yield return null;
            }
        }
    }
}