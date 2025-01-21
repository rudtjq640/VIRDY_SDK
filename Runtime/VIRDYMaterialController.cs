using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(NetworkObject))]
    public class VIRDYMaterialController : VIRDYNetworkBehaviour
    {
        [SerializeField]
        private Material _material;

        [Space]

        [SerializeField]
        private List<VIRDYMaterialSlider> _sliders = new List<VIRDYMaterialSlider>() { };
        [SerializeField]
        private List<VIRDYMaterialFloat> _floats = new List<VIRDYMaterialFloat>() { };

        protected override void OnInitialize()
        {
            if (_material == null)
                return;

            // sliders
            for (int i = 0; i < _sliders.Count; i++)
            {
                int index = i;
                var onChanged = _sliders[index].Initialize(_material);
                if (onChanged != null)
                {
                    onChanged.AddListener((value) => RPC_OnSliderValueChanged(index, value));
                }
            }
            // floats
            for (int i = 0; i < _floats.Count; i++)
            {
                int index = i;
                var onChanged = _floats[index].Initialize(_material);
                if (onChanged != null)
                {
                    onChanged.AddListener((value) =>
                    {
                        if (float.TryParse(value, out float f))
                            RPC_OnFloatValueChanged(index, f);
                    });
                }
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Unreliable)]
        public void RPC_OnSliderValueChanged(int index, float value)
        {
            _sliders[index].SetValue(value);
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
        public void RPC_OnFloatValueChanged(int index, float value)
        {
            _floats[index].SetValue(value);
        }

        public void ResetValue()
        {
            for (int i = 0; i < _sliders.Count; i++) _sliders[i].ResetValue();
            for (int i = 0; i < _floats.Count; i++) _floats[i].ResetValue();
        }
    }
}
