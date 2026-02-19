using UnityEngine;
using System.Collections;

namespace VIRDY.SDK
{

    public class VIRDYPaperAero : MonoBehaviour
    {
        public Rigidbody rb;

        [Header("Air Properties")]
        public float airDensity = 6.0f;  // 공기밀도
        public float area = 0.015f;      // 종이 투영 면적
        public float Cd = 1.2f;          // 항력 계수
        public float Cl = 0.8f;          // 리프트 계수(간이) - 각도에 따라 가변

        [Header("Flutter / Torque")]
        public float forceOffset = 0.05f;    // 힘 가하는 지점 오프셋(월드, m) - 윗모서리 쪽으로
        public Vector3 offsetDir = Vector3.up;
        public float flutterGain = 0.35f;    // 팔랑 토크 크기(속도 의존)
        public float turbulence = 0.8f;      // 난류(퍼린/랜덤) 토크 강도
        public float turbulenceFreq = 3f;    // 난류 주파수(Hz)

        [Header("Gravity Scaling")]
        [Tooltip("실제 중력보다 가볍게 느껴지도록 0~1 사이로 줄임 (0.4~0.7 추천)")]
        [Range(0f, 1.5f)] public float gravityScale = 0.1f;

        [Header("Tuning")]
        public Transform paperNormalRef; // 종이 면의 법선 기준(없으면 this)
        public Vector3 normalLocal = new Vector3(0, 0.8f, 0.2f);    // 종이 평면 법선(로컬)
        public bool drawGizmos = false;

        float noiseSeed;

        void Reset()
        {
            rb = GetComponent<Rigidbody>();
        }

        void OnEnable()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            noiseSeed = Random.value * 1000f;
        }

        void FixedUpdate()
        {
            if (!rb) TryGetComponent(out rb);
            if (!rb) return;

            // 1) 효과적 중력: 중력 스케일 다운
            Vector3 g = Physics.gravity * rb.mass;
            Vector3 scaledGravity = g * gravityScale;
            // 실제 중력은 이미 작용하므로, 차이만큼 역방향으로 보정 force 적용
            rb.AddForce((scaledGravity - g), ForceMode.Force);

            // 2) 공기 상대 속도 (공기 정지 가정)
            Vector3 v = rb.velocity;
            float speed = v.magnitude;
            if (speed < 0.01f) return;
            Vector3 vHat = v / speed;

            // 종이 법선
            Transform refTr = paperNormalRef ? paperNormalRef : transform;
            Vector3 n = refTr.TransformDirection(normalLocal).normalized;

            // 3) 항력 (속도 방향 반대)
            float q = 0.5f * airDensity * speed * speed;   
            Vector3 Fdrag = -vHat * (q * Cd * area);
            // 힘 가할 위치(윗모서리 쪽)
            Vector3 fp = transform.position + refTr.TransformDirection(offsetDir) * forceOffset;
            rb.AddForceAtPosition(Fdrag, fp, ForceMode.Force);

            // 4) 리프트 (간이): 유속과 법선의 교차로 측면 방향 계산
            //   - 평판의 받음각에 따른 lift를 단순화하여 사용
            float cosTheta = Mathf.Clamp(Vector3.Dot(-vHat, n), -1f, 1f); // 유속과 법선의 상대각
            float sinTheta = Mathf.Sqrt(Mathf.Max(0f, 1f - cosTheta * cosTheta));
            Vector3 liftDir = Vector3.Cross(vHat, Vector3.Cross(n, vHat)).normalized; // 평면에 수직이고 v에 수직인 방향
            Vector3 Flift = liftDir * (q * Cl * area * sinTheta); // 각도 클수록 리프트 증가
            rb.AddForceAtPosition(Flift, fp, ForceMode.Force);

            // 5) 팔랑 토크: 속도가 있을 때 법선과 유속 관계에 따라 뒤집히려는 토크 부여
            //    v에 수직이며 법선 회전 성분을 만드는 토크
            Vector3 flutterAxis = Vector3.Cross(n, vHat).normalized; // 뒤집히는 축
            float flutterTorqueMag = flutterGain * q * area; // v^2 의존
            rb.AddTorque(flutterAxis * flutterTorqueMag, ForceMode.Force);

            // 6) 난류 토크: 페린 노이즈로 살짝 불규칙성 부여
            float t = Time.time + noiseSeed;
            float nx = Mathf.PerlinNoise(t * turbulenceFreq, 0.123f) * 2f - 1f;
            float ny = Mathf.PerlinNoise(0.456f, t * turbulenceFreq) * 2f - 1f;
            float nz = Mathf.PerlinNoise(t * turbulenceFreq, t * turbulenceFreq) * 2f - 1f;
            Vector3 noise = new Vector3(nx, ny, nz).normalized * (turbulence * 0.02f * q * area);
            rb.AddTorque(noise, ForceMode.Force);
        }

        void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;
            Transform refTr = paperNormalRef ? paperNormalRef : transform;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + refTr.TransformDirection(offsetDir) * forceOffset);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + refTr.TransformDirection(normalLocal) * 0.1f);
        }
    }
}
