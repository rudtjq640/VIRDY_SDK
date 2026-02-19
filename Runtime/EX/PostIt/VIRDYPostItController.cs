using System.Collections.Generic;
using UnityEngine;

namespace VIRDY.SDK
{
    [System.Serializable]
    public struct PostItData
    {
        public PostItStickyController controller;
        public float DetachThreshold;
    }
    public class VIRDYPostItController : MonoBehaviour
    {
        [Header("Stabilization")]
        public float minHoldTime = 0.15f;
        public float cooldown = 0.15f;
        public float warmupIgnore = 0.25f;

        [Header("Sampling")]
        [Tooltip("샘플 간격(초). 0이면 매 호출마다 샘플")]
        public float sampleInterval = 0f;

        [Header("Targets")]
        public List<PostItData> controllers = new List<PostItData>();

        [Header("Initial State")]
        public bool startActive = false;
        public bool forceDisableOnStart = false;

        public event System.Action<int> OnDetached;

        // runtime
        bool _runtimeActive;
        double _nextSampleAt = 0.0;

        void Start()
        {
            _runtimeActive = startActive && !forceDisableOnStart;

            if (_runtimeActive) ActivateAllInternal();
            else DeactivateAllInternal();

            for (int i = 0; i < controllers.Count; i++)
            {
                int idx = i;
                var c = controllers[idx].controller;
                if (!c) continue;
                c.OnDetached += () => OnDetached?.Invoke(idx);
            }
        }

        // ─────────────────────────────────────────────
        // 외부(부모 NetworkActor)가 WriteBuffer 타이밍에 호출
        // ─────────────────────────────────────────────
        public void ManagerTick(float dt, double now)
        {
            if (!_runtimeActive) return;
            if (dt <= 0f) return;

            if (sampleInterval > 0f)
            {
                if (now + 1e-6 < _nextSampleAt) return; // 샘플 주기 미도달
                _nextSampleAt = now + sampleInterval;
            }

            // 모든 포스트잇에 동일 dt/now로 측정
            for (int i = 0; i < controllers.Count; i++)
            {
                var c = controllers[i].controller;
                if (!c || !c.enabled) continue;
                c.Tick(dt, now);
            }
        }

        // ======= Public API =======
        public void ActivateAll()
        {
            _runtimeActive = true;
            ActivateAllInternal();
        }

        public void DeactivateAll()
        {
            _runtimeActive = false;
            DeactivateAllInternal();
        }

        public void SetAllActive(bool active)
        {
            if (active) ActivateAll();
            else DeactivateAll();
        }

        public void RefreshOnce(float dt, double now)
        {
            if (!_runtimeActive) return;
            for (int i = 0; i < controllers.Count; i++)
            {
                var c = controllers[i].controller;
                if (c && c.enabled) c.Tick(dt, now);
            }
            _nextSampleAt = now + Mathf.Max(0f, sampleInterval);
        }

        public void Detached(int index)
        {
            if (index < 0 || index >= controllers.Count)
            {
                Debug.LogWarning($"[PostIt] Detached index out of range: {index}");
                return;
            }
            var c = controllers[index];
            if (!c.controller)
            {
                Debug.LogWarning($"[PostIt] Controller at {index} is null");
                return;
            }
            c.controller.DetachNow();
        }

        // ======= Internals =======
        void ActivateAllInternal()
        {
            foreach (var c in controllers)
            {
                if (!c.controller) continue;
                c.controller.DetachThreshold = c.DetachThreshold;
                c.controller.ActivateAndStick(null);
                c.controller.enabled = true;
            }
            _nextSampleAt = 0.0; // 즉시 샘플 가능
        }

        void DeactivateAllInternal()
        {
            foreach (var c in controllers)
            {
                if (!c.controller) continue;
                c.controller.ForceResetAndDisable();
                c.controller.enabled = false;
            }
        }
    }
}
