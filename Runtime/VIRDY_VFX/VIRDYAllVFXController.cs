using UnityEngine;
using UnityEngine.VFX;

public class VIRDYAllVFXController : MonoBehaviour
{
    private VisualEffect[] vfxArray;
    
    private void Start()
    {
        vfxArray = GetComponentsInChildren<VisualEffect>();
    }

    public void PlayAllVFX()
    {
        foreach (var vfx in vfxArray)
        {
            vfx.Play();
        }
    }

    public void StopAllVFX()
    {
        foreach (var vfx in vfxArray)
        {
            vfx.Stop();
        }
    }
}
