using UnityEngine;
using UnityEngine.Video;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VIRDYVideoPlayer : VIRDYBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private VideoClip[] _videoClips;

        [SerializeField]
        private RenderTexture[] _renderTextures;

        [SerializeField]
        private VIRDYFadeUI[] _fadeUIs;

        private VideoPlayer _videoPlayer;

        protected override void OnInitialize()
        {
            _videoPlayer = this.GetComponent<VideoPlayer>();
        }

        public void Play()
        {
            _videoPlayer.Play();
        }

        public void Pause()
        {
            _videoPlayer.Pause();
        }

        public void Stop()
        {
            _videoPlayer.frame = 0;
            _videoPlayer.Stop();
        }

        public void PlayClip(int index)
        {
            Stop();
            SetClip(index);
            Play();
            FadeOutAll();
            FadeIn(index);
        }

        public void StopClip()
        {
            Stop();
            FadeOutAll();
        }

        public void SetClip(int index)
        {
            if ((_videoClips?.Length ?? 0) >= index + 1)
            {
                _videoPlayer.clip = _videoClips[index];
            }
            if ((_renderTextures?.Length ?? 0) >= index + 1)
            {
                ClearRenderTexture(_renderTextures[index]);
                _videoPlayer.targetTexture = _renderTextures[index];
            }

            SetTargetAudioSource();
        }

        public void SetVolume(float value)
        {
            _audioSource.volume = Mathf.Clamp01(value);
        }

        public void SetTargetAudioSource()
        {
            if (_videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
            {
                _videoPlayer.EnableAudioTrack(0, true);
                _videoPlayer.SetTargetAudioSource(0, _audioSource);
            }
        }

        public void FadeOutAll()
        {
            foreach (var fadeUI in _fadeUIs) fadeUI?.FadeOut();
        }

        public void FadeIn(int index)
        {
            _fadeUIs[index].FadeIn();
        }

        public void FadeOut(int index)
        {
            _fadeUIs[index].FadeOut();
        }

        private static void ClearRenderTexture(RenderTexture renderTexture)
        {
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
        }
    }
}
