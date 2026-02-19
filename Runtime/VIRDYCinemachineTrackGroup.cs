using UnityEngine;
using UnityEngine.Playables;

public class VIRDYCinemachineTrackGroup : MonoBehaviour
{
    public void StopAllTimelines()
    {
        var directors = GetComponentsInChildren<PlayableDirector>();
        foreach (var director in directors)
        {
            director.Stop();
        }
    }
}
