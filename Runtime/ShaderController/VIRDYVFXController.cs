using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
public class VIRDYVFXController : NetworkBehaviour
{
    [SerializeField]
    private VisualEffect _visualEffect;

    [Space]

    [SerializeField]
    private Gradient[] _gradients;

    [Space]

    [SerializeField]
    private List<VIRDYVFXSlider> _sliders = new List<VIRDYVFXSlider>() { };
    [SerializeField]
    private List<VIRDYVFXFloat> _floats = new List<VIRDYVFXFloat>() { };
    [SerializeField]
    private List<VIRDYVFXVector2> _vector2s = new List<VIRDYVFXVector2>() { };

    private void Start()
    {
        // sliders
        for (int i = 0; i < _sliders.Count; i++)
        {
            int index = i;
            _sliders[index].Initialize(_visualEffect).AddListener((value) => RPC_OnSliderValueChanged(index, value));
        }
        // floats
        for (int i = 0; i < _floats.Count; i++)
        {
            int index = i;
            _floats[index].Initialize(_visualEffect).AddListener((value) =>
            {
                if (float.TryParse(value, out float f))
                    RPC_OnFloatValueChanged(index, f);
            });

        }
        // vector2s
        for (int i = 0; i < _vector2s.Count; i++)
        {
            int index = i;
            _vector2s[index].Initialize(_visualEffect).AddListener((value) => RPC_OnVector2ValueChanged(index, value));
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

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_OnVector2ValueChanged(int index, Vector2 value)
    {
        _vector2s[index].SetValue(value);
    }

    public void ResetValue()
    {
        for (int i = 0; i < _sliders.Count; i++) _sliders[i].ResetValue();
        for (int i = 0; i < _floats.Count; i++) _floats[i].ResetValue();
        for (int i = 0; i < _vector2s.Count; i++) _vector2s[i].ResetValue();
    }

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_SetGradientColor(int index)
    {
        if (_visualEffect.HasGradient("Color") == false)
            return;

        _visualEffect.SetGradient("Color", _gradients[index]);
    }
}
