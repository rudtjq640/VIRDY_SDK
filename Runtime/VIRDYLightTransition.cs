using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VIRDY.SDK
{
    [Serializable]
    public class LightPreset
    {
        public LightColorSet[] LightColorSets;

        public MaterialColorSet[] MaterialColorSets;

        public float Duration;

        [Serializable]
        public class LightColorSet
        {
            public Color Color;
            public float Intensity;
            public float Range;
        }

        [Serializable]
        public class MaterialColorSet
        {
            public Color Color;
            public string PropertyName;
        }
    }

    public class VIRDYLightTransition : VIRDYBehaviour
    {
        [SerializeField]
        private LightPreset[] _presets;

        [SerializeField]
        private Light[] _lights;

        [SerializeField]
        private Material[] _materials;

        private Coroutine _current;
        
        [Space]

        [SerializeField]
        private UnityEvent _onUpdate = new UnityEvent();

        public void SetPreset(int presetIndex)
        {
            if (presetIndex < 0 || presetIndex >= _presets.Length) return;

            if (_current != null) StopCoroutine(_current);
            _current = StartCoroutine(DoTransition(presetIndex));
        }

        private IEnumerator DoTransition(int presetIndex)
        {
            float elapsedTime = 0f;

            List<Color> light_aColor = new List<Color>();
            List<float> light_aIntensity = new List<float>();
            List<float> light_aRange = new List<float>();

            List<Color> material_aColor = new List<Color>();

            for (int i = 0; i < _lights.Length; i++)
            {
                light_aColor.Add(_lights[i].color);
                light_aIntensity.Add(_lights[i].intensity);
                light_aRange.Add(_lights[i].range);
            }
            for (int i = 0; i < _materials.Length; i++)
            {
                material_aColor.Add(_materials[i].GetColor(_presets[presetIndex].MaterialColorSets[i].PropertyName));
            }

            while (elapsedTime < _presets[presetIndex].Duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / _presets[presetIndex].Duration;

                for (int i = 0; i < _lights.Length; i++)
                {
                    _lights[i].color = Color.Lerp(light_aColor[i], _presets[presetIndex].LightColorSets[i].Color, t);
                    _lights[i].intensity = Mathf.Lerp(light_aIntensity[i], _presets[presetIndex].LightColorSets[i].Intensity, t);
                    _lights[i].range = Mathf.Lerp(light_aRange[i], _presets[presetIndex].LightColorSets[i].Range, t);
                }

                for (int i = 0; i < _materials.Length; i++)
                {
                    _materials[i].SetColor(_presets[presetIndex].MaterialColorSets[i].PropertyName, Color.Lerp(material_aColor[i], _presets[presetIndex].MaterialColorSets[i].Color, t));
                }

                _onUpdate?.Invoke();

                yield return null;
            }

            _current = null;
        }
    }
}
