using UnityEngine;
using System.Collections;

namespace VIRDY.SDK
{
    /// <summary>
    /// 네트워크 비의존. 외부 매니저가 Tick(dt, now)만 호출하는 순수 감지기.
    /// * 워밍업/지속판정은 dt 누적 기반으로 구현 → 유니티/퓨전 시계 섞여도 안전
    /// </summary>
    [DisallowMultipleComponent]
    public class PostItStickyController : MonoBehaviour
    {
        [Header("Target & Ground")]
        [Tooltip("흔들림을 측정할 본(부위). ActivateAndStick 호출 시 갱신됨")]
        public Transform targetBone;
        [Tooltip("Ground로 인식할 태그명")]
        public string groundTag = "Ground";

        [Header("Shake Detection (High-pass + RMS)")]
        [Tooltip("하이패스 시간상수(초). 작을수록 고주파에 민감(0.08~0.2 권장)")]
        public float hpTau = 0.12f;

        [Header("RMS (Time-constant)")]
        [Tooltip("RMS(EMA) 시간상수(초)")]
        public float rmsTau = 0.18f;

        [Tooltip("가중치: 선형가속도 / 각속도 / 각가속도")]
        public float wLinearAcc = 1.0f;
        public float wAngularVel = 0.7f;
        public float wAngularAcc = 1.2f;

        [Header("Thresholds & Timings")]
        [Tooltip("떨어질 정도의 흔들림 에너지 임계값")]
        public float DetachThreshold = 780.0f;
        [Tooltip("임계치에 더해지는 랜덤 지터(테스트 편의). 0이면 비활성")]
        public float detachJitter = 5.0f;
        [Tooltip("재부착/유지용 히스테리시스 비율(임계값 대비)")]
        public float attachRatio = 0.6f;
        [Tooltip("임계 이상이 유지되어야 하는 최소 시간(초)")]
        public float sustainTime = 0.10f;
        [Tooltip("활성화 직후 흔들림 감지를 무시하는 시간(초)")]
        public float warmupIgnoreTime = 0.05f;

        [Header("Detach Behavior")]
        [Tooltip("떨어질 때 표면 법선 방향 팝 임펄스(0이면 비활성)")]
        public float popImpulse = 0.4f;
        public Vector3 surfaceNormal = Vector3.zero;
        public ForceMode popForceMode = ForceMode.Impulse;

        [Header("Reset Behavior")]
        [Tooltip("바닥에 닿은 뒤 리셋까지 지연(초)")]
        public float resetDelay = 1.5f;
        [Tooltip("리셋 시 속도·회전 완전 정지")]
        public bool zeroVelOnReset = true;
        [Tooltip("리셋 시 Rigidbody를 파괴(권장). false면 키네마틱/무중력으로 전환")]
        public bool destroyRbOnReset = true;
        [Tooltip("붙어있는 동안 키네마틱/무중력으로 유지")]
        public bool kinematicWhileStuck = true;

        [Header("Space")]
        [Tooltip("로컬 공간(본 로컬 포즈 기반) vs 월드 공간(권장)")]
        public bool useLocalSpace = false;

        // ====== Rigidbody 인스펙터 제어 ======
        [System.Serializable]
        public class RigidbodyInspector
        {
            public bool applySettings = true;
            public bool useGravity = true;
            [Min(0f)] public float mass = 0.02f;
            [Min(0f)] public float drag = 0.05f;
            [Min(0f)] public float angularDrag = 0.05f;
            public bool isKinematic = false;
            public RigidbodyInterpolation interpolation = RigidbodyInterpolation.Interpolate;
            public CollisionDetectionMode collisionDetection = CollisionDetectionMode.ContinuousSpeculative;
            public RigidbodyConstraints constraints = RigidbodyConstraints.None;

            public void ApplyTo(Rigidbody rb)
            {
                if (!applySettings || !rb) return;
                rb.useGravity = useGravity;
                rb.mass = Mathf.Max(0.0001f, mass);
                rb.drag = Mathf.Max(0f, drag);
                rb.angularDrag = Mathf.Max(0f, angularDrag);
                rb.isKinematic = isKinematic;
                rb.interpolation = interpolation;
                rb.collisionDetectionMode = collisionDetection;
                rb.constraints = constraints;
            }
        }
        [Header("Rigidbody Inspector")]
        public RigidbodyInspector rigidbodyInspector = new RigidbodyInspector();

        public event System.Action OnDetached;

        // origin
        Transform originalParent;
        Vector3 originalLocalPos;
        Quaternion originalLocalRot;
        bool hasOrigin;

        // runtime
        Rigidbody rb;
        Collider col;

        Vector3 prevPos;
        Quaternion prevRot;
        Vector3 vel, prevVel, acc;
        Vector3 angVel, prevAngVel, angAcc;

        Vector3 hpf_acc, prev_acc;
        Vector3 hpf_angVel, prev_angVel;
        Vector3 hpf_angAcc, prev_angAcc;

        float rmsEnergy;
        float warmupElapsed = 0f; // 워밍업 누적
        float overTimer = 0f;     // 임계 초과 누적
        bool isDetached;

        float detachThreshold;    // 실제 사용 임계

        VIRDYPaperAero paperAero;

        void Awake()
        {
            col = GetComponent<Collider>();
            rb  = GetComponent<Rigidbody>();
            paperAero = GetComponent<VIRDYPaperAero>();
        }

        void OnEnable()
        {
            if (!hasOrigin)
            {
                originalParent = transform.parent;
                originalLocalPos = transform.localPosition;
                originalLocalRot = transform.localRotation;
                hasOrigin = true;
            }

            if (!targetBone) targetBone = transform.parent ? transform.parent : transform;

            ResetDetectionState();

            if (rb && kinematicWhileStuck)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                if (zeroVelOnReset)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        /// <summary>외부 매니저가 시뮬/렌더 타이밍에서 호출</summary>
        public void Tick(float dt, double now)
        {
            Step(dt);
        }

        // ====== 공개 API ======
        public void ActivateAndStick(Transform bone)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            if (!hasOrigin)
            {
                originalParent = transform.parent;
                originalLocalPos = transform.localPosition;
                originalLocalRot = transform.localRotation;
                hasOrigin = true;
            }

            if (bone) targetBone = bone;

            transform.SetParent(originalParent, false);
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;

            ResetDetectionState();

            if (rb)
            {
                if (zeroVelOnReset)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                if (kinematicWhileStuck)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
        }

        public void ForceResetAndDisable()
        {
            StopAllCoroutines();
            ResetToOriginPose();

            if (rb)
            {
                if (zeroVelOnReset)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                if (destroyRbOnReset)
                {
                    Destroy(rb);
                    rb = null;
                }
                else if (kinematicWhileStuck)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }

            ResetDetectionState();
            gameObject.SetActive(false);
        }

        public void DetachNow()
        {
            if (isDetached) return;
            DoDetach();
        }

        // ====== 핵심 로직 ======
        void Step(float dt)
        {
            if (!targetBone || isDetached || dt <= 0f) return;

            // 워밍업: 절대시간 비교 제거, dt 누적
            if (warmupIgnoreTime > 0f && warmupElapsed < warmupIgnoreTime)
            {
                warmupElapsed += dt;
                SamplePose(out prevPos, out prevRot);
                return;
            }

            Vector3 currPos; Quaternion currRot;
            SamplePose(out currPos, out currRot);

            vel = (currPos - prevPos) / dt;
            acc = (vel - prevVel) / dt;

            Quaternion dq = currRot * Quaternion.Inverse(prevRot);
            dq.ToAngleAxis(out float angleDeg, out Vector3 axis);
            if (float.IsNaN(axis.x)) axis = Vector3.zero;
            float angleRad = Mathf.Deg2Rad * Mathf.DeltaAngle(0f, angleDeg);
            angVel = (axis * angleRad) / dt;
            angAcc = (angVel - prevAngVel) / dt;

            // HPF (시간상수형)
            float alphaHP = hpTau / Mathf.Max(1e-4f, (hpTau + dt));
            hpf_acc    = alphaHP * (hpf_acc    + acc    - prev_acc);
            hpf_angVel = alphaHP * (hpf_angVel + angVel - prev_angVel);
            hpf_angAcc = alphaHP * (hpf_angAcc + angAcc - prev_angAcc);

            prev_acc    = acc;
            prev_angVel = angVel;
            prev_angAcc = angAcc;

            float shake =
                wLinearAcc * hpf_acc.magnitude +
                wAngularVel * hpf_angVel.magnitude +
                wAngularAcc * hpf_angAcc.magnitude;

            float power = shake * shake;

            // RMS (시간상수형)
            float a = 1f - Mathf.Exp(-dt / Mathf.Max(1e-4f, rmsTau));
            rmsEnergy += (power - rmsEnergy) * a;

            // 임계 비교 (제곱공간)
            float det  = detachThreshold;
            float det2 = det * det;

            if (rmsEnergy >= det2)
            {
                overTimer += dt; // 임계 초과 누적
                if (overTimer >= sustainTime)
                    DoDetach();
            }
            else
            {
                float att = attachRatio * det;
                float att2 = att * att;
                if (rmsEnergy <= att2)
                    overTimer = 0f; // 충분히 가라앉으면 초기화
            }

            prevPos = currPos;
            prevRot = currRot;
            prevVel = vel;
            prevAngVel = angVel;
        }

        void DoDetach()
        {
            isDetached = true;

            transform.SetParent(null, true);

            if (!rb) rb = gameObject.AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.useGravity = true;

            rigidbodyInspector.ApplyTo(rb);

            if (rb && surfaceNormal != Vector3.zero && popImpulse > 0f)
                rb.AddForce(surfaceNormal.normalized * popImpulse, popForceMode);

            if (paperAero) paperAero.enabled = true;

            OnDetached?.Invoke();
        }

        void OnCollisionEnter(Collision other)
        {
            if (!isDetached) return;

            bool isGround = other.collider.gameObject.name.Contains(groundTag);
            if (isGround)
                StartCoroutine(ResetAfterDelay());
        }

        IEnumerator ResetAfterDelay()
        {
            yield return new WaitForSeconds(resetDelay);

            ResetToOriginPose();

            if (rb)
            {
                if (zeroVelOnReset)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                if (destroyRbOnReset)
                {
                    Destroy(rb);
                    rb = null;
                }
                else if (kinematicWhileStuck)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }

            ResetDetectionState();
            if (paperAero) paperAero.enabled = false;
            gameObject.SetActive(false);
        }

        void ResetToOriginPose()
        {
            if (!hasOrigin) return;
            transform.SetParent(originalParent, false);
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;
        }

        void ResetDetectionState()
        {
            float jitter = Mathf.Max(0f, detachJitter);
            detachThreshold = Mathf.Max(0f, DetachThreshold) + (jitter > 0f ? Random.Range(0.0f, jitter) : 0f);

            isDetached = false;
            warmupElapsed = 0f;
            overTimer = 0f;
            rmsEnergy = 0f;

            SamplePose(out prevPos, out prevRot);
            vel = prevVel = Vector3.zero;
            angVel = prevAngVel = Vector3.zero;
            acc = prev_acc = Vector3.zero;
            hpf_acc = hpf_angVel = hpf_angAcc = Vector3.zero;
            prev_angAcc = Vector3.zero;
        }

        void SamplePose(out Vector3 pos, out Quaternion rot)
        {
            if (!targetBone) targetBone = transform.parent ? transform.parent : transform;
            if (useLocalSpace)
            {
                pos = targetBone.localPosition;
                rot = targetBone.localRotation;
            }
            else
            {
                pos = targetBone.position;
                rot = targetBone.rotation;
            }
        }
    }
}
