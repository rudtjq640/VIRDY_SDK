using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace VIRDY.SDK
{
    public class VIRDYCinemachineTrack : VIRDYBehaviour
    {
        public static UnityAction<Camera> OnMainCameraChange;
        public static GetMainCameraDelegate GetMainCamera;

        public delegate Camera GetMainCameraDelegate();

        [SerializeField]
        private PlayableDirector _playableDirector;

        [SerializeField]
        private string _cinemachineTrackName = "Cinemachine Track";

        protected override void OnInitialize()
        {
#if VIRDY_CORE
            if (GetMainCamera != null)
            {
                OnMainCameraChanged(GetMainCamera());
            }
#endif
            OnMainCameraChange += OnMainCameraChanged;
        }
        protected override void OnDeinitialize()
        {
            OnMainCameraChange -= OnMainCameraChanged;
        }

        private void OnMainCameraChanged(Camera camera)
        {
            var brain = camera.GetComponent<CinemachineBrain>();
            if (brain == null) return;

            var timelineAsset = _playableDirector.playableAsset as TimelineAsset;
            if (timelineAsset == null) return;

            var trackAssets = timelineAsset.GetOutputTracks().Where(e => e.name.Equals(_cinemachineTrackName));
            foreach (var cinemachineTrack in trackAssets.Where(asset => asset.GetType().Equals(typeof(CinemachineTrack))))
            {
                var boundBrain = _playableDirector.GetGenericBinding(cinemachineTrack) as CinemachineBrain;
                if (boundBrain != null)
                    _playableDirector.SetGenericBinding(cinemachineTrack, brain);
            }
        }
    }
}
