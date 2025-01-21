using System.Collections;
using UnityEngine;
using Fusion;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(NetworkObject))]
    public class VIRDYRoulette : VIRDYNetworkBehaviour
    {
        #region Private Members

        [SerializeField]
        private Transform _roulette;

        [Space]

        [SerializeField]
        private int _minShuffleCount = 10;
        [SerializeField]
        private int _maxShuffleCount = 30;

        [Space]

        [SerializeField]
        private float _shuffleSpeed = 1f;

        [SerializeField]
        private AnimationCurve[] _animationCurves;

        private float _currentProgress;

        private float _startTime;

        private Coroutine _current;

#if UNITY_EDITOR
        [SerializeField]
        private bool _startShuffle;
#endif

        #endregion

        #region Networked Properties

        [Networked(OnChanged = nameof(OnProgressChanged))]
        private float _targetProgress { get; set; }
        public float TargetProgress
        {
            set
            {
                _targetProgress = value;
                if (Object == false) OnProgressChanged();
            }
        }

        public static void OnProgressChanged(Changed<VIRDYRoulette> changed)
        {
            changed.Behaviour.OnProgressChanged();
        }

        private void OnProgressChanged()
        {
            if (_targetProgress == _currentProgress) return;

            if (_current != null) StopCoroutine(_current);
            _current = StartCoroutine(Shuffle(_shuffleTime, _targetProgress, _curveIndex));
        }

        [Networked]
        private int _shuffleTime { get; set; }

        [Networked]
        private int _curveIndex { get; set; }

        #endregion

        #region NetworkBehaviour Interface

        public override void Spawned()
        {
            UpdateAngle(_targetProgress);
        }

        #endregion

        #region Public Methods

#if VIRDY_CORE
        public async void StartShuffle()
#else
        public void StartShuffle()
#endif
        {
#if VIRDY_CORE
            if (Object.HasStateAuthority == false) Object.RequestStateAuthority();
            await System.Threading.Tasks.Task.Delay(100);
#endif

            _shuffleTime = Mathf.Max(0, Random.Range(_minShuffleCount, _maxShuffleCount));
            _curveIndex = Random.Range(0, _animationCurves.Length);
            TargetProgress = (_currentProgress % 1) + Random.Range(0f, 1f) + _shuffleTime;
        }

#endregion

        #region Private Methods

#if UNITY_EDITOR
        private void Update()
        {
            if (_startShuffle == true) StartShuffle();
            _startShuffle = false;
        }
#endif

        private IEnumerator Shuffle(float shuffleTime, float tProgress, int randomCurve)
        {
            float pProgress = _currentProgress % 1;

            _startTime = Time.time;
            var time = 0f;

            while (time < 1f)
            {
                time = (Time.time - _startTime) / shuffleTime * _shuffleSpeed;

                var absProgress = tProgress - pProgress;
                var progress = _animationCurves[randomCurve].Evaluate(time) * absProgress + pProgress;
                UpdateAngle(progress);

                yield return null;
            }

            _current = null;
        }

        private void UpdateAngle(float value)
        {
            _currentProgress = value;
            _roulette.localEulerAngles = new Vector3(_roulette.localEulerAngles.x, _roulette.localEulerAngles.y, 360f * value);
        }

        #endregion
    }
}
