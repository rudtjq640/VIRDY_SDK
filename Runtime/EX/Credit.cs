using UnityEngine;
using Fusion;

namespace VIRDY.SDK
{
    public class Credit : VIRDYNetworkBehaviour
    {
        [Networked]
        public Vector2 _anchoredPosition { get; set; }

        private Interpolator<Vector2> _posInterpolator;

        [SerializeField]
        private float _speed = 1f;

        private RectTransform _rectTransform;

        private bool _isPlaying;

        private bool _rewind;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _rectTransform = this.GetComponent<RectTransform>();
        }

        public void Update()
        {
#if VIRDY_CORE
            if ((Object?.HasStateAuthority ?? false) == false) return;
#endif

            if (_isPlaying == false) return;

            this.transform.Translate(Vector3.left * _speed * Time.deltaTime * (_rewind ? -1f : 1f) * 200f);
        }

        public override void Spawned()
        {
            base.Spawned();

            _posInterpolator = GetInterpolator<Vector2>(nameof(_anchoredPosition));
        }

        public override void FixedUpdateNetwork()
        {
            if ((Object?.HasStateAuthority ?? false) == false) return;

            _anchoredPosition = _rectTransform.anchoredPosition;
        }

        public override void Render()
        {
            if ((Object?.HasStateAuthority ?? true) == true) return;

            _rectTransform.anchoredPosition = _posInterpolator.Value;
        }

        public async void Play()
        {
#if VIRDY_CORE
            if (Object.HasStateAuthority == false) Object.RequestStateAuthority();
            await System.Threading.Tasks.Task.Delay(100);
#endif

            RPC_Play();
        }

        public async void Rewind()
        {
#if VIRDY_CORE
            if (Object.HasStateAuthority == false) Object.RequestStateAuthority();
            await System.Threading.Tasks.Task.Delay(100);
#endif

            RPC_Rewind();
        }

        public async void Stop()
        {
#if VIRDY_CORE
            if (Object.HasStateAuthority == false) Object.RequestStateAuthority();
            await System.Threading.Tasks.Task.Delay(100);
#endif

            RPC_Stop();
        }

        public async void ResetPosition()
        {
#if VIRDY_CORE
            if (Object.HasStateAuthority == false) Object.RequestStateAuthority();
            await System.Threading.Tasks.Task.Delay(100);
#endif

            _rectTransform.anchoredPosition = new Vector3(0f, _rectTransform.anchoredPosition.y);
        }

        public async void MoveUp()
        {
#if VIRDY_CORE
            if (Object.HasStateAuthority == false) Object.RequestStateAuthority();
            await System.Threading.Tasks.Task.Delay(100);
#endif

            _rectTransform.anchoredPosition = new Vector3(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.y + 20f);
        }

        public async void MoveDown()
        {
#if VIRDY_CORE
            if (Object.HasStateAuthority == false) Object.RequestStateAuthority();
            await System.Threading.Tasks.Task.Delay(100);
#endif

            _rectTransform.anchoredPosition = new Vector3(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.y - 20f);
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
        private void RPC_Play()
        {
            _isPlaying = true;
            _rewind = false;
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
        private void RPC_Rewind()
        {
            _isPlaying = true;
            _rewind = true;
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
        private void RPC_Stop()
        {
            _isPlaying = false;
        }
    }
}
