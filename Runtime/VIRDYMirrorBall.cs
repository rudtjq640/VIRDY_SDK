using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VIRDY.SDK
{
    public class VIRDYMirrorBall : MonoBehaviour
    {
        public List<Light> mirrorBallLights; // Light 리스트 추가
        public float intensity = 1f; // Intensity 값
        public float hueChangeSpeed = 1f; // H 값 변화 속도
        public float saturation = 0.15f; // S 값
        public float value = 1f; // V 값
        public float rotateSpeed = 50f; // Rotate 속도

        private bool isPlaying = false; // 현재 애니메이션을 재생 중인지 여부
        private Coroutine currentAnimation; // 현재 재생 중인 애니메이션 Coroutine 참조

        private float currentRotateSpeed; // 현재 Rotate 속도

        void Start()
        {
            // Start 시 애니메이션 재생하지 않음
            StopAnimation();
            foreach (var light in mirrorBallLights)
            {
                light.intensity = intensity; // Intensity 값을 컴포넌트 설정 값으로 적용
            }
        }

        public void MusicStart() // Light ON 메서드 호출
        {
            if (!isPlaying)
            {
                // 현재 애니메이션을 중지, 새로운 애니메이션 재생
                StopAnimation();
                foreach (var light in mirrorBallLights)
                {
                    light.intensity = intensity;    // Intensity 값을 컴포넌트에서 설정된 값으로 변경
                }
                currentAnimation = StartCoroutine(PlayAnimation());
            }
        }

        public void MusicStop() // Light OFF 메서드 호출
        {
            if (isPlaying)
            {
                // 현재 애니메이션을 중지, 새로운 애니메이션 재생
                StopAnimation();
                foreach (var light in mirrorBallLights)
                {
                    light.intensity = 0f; // Intensity 0 으로 변경해서 종료
                }
                currentAnimation = StartCoroutine(ReverseAnimation());
            }
        }

        // 애니메이션 재생 Coroutine
        private IEnumerator PlayAnimation()
        {
            isPlaying = true;

            // 시작 속도 설정
            currentRotateSpeed = rotateSpeed;

            // 색상 변화 애니메이션
            while (isPlaying)
            {
                // H 값 변화
                float hue = (Time.time * hueChangeSpeed) % 1f;
                Color color = Color.HSVToRGB(hue, saturation, value);
                foreach (var light in mirrorBallLights)
                {
                    light.color = color;
                }

                foreach (var light in mirrorBallLights)
                {
                    light.transform.Rotate(Vector3.up * Time.deltaTime * currentRotateSpeed);
                }

                yield return null;
            }
        }

        // 애니메이션 역재생 Coroutine
        private IEnumerator ReverseAnimation()
        {
            // Rotate 애니메이션 멈춤
            isPlaying = false;

            // 색상 변화 애니메이션
            while (mirrorBallLights[0].intensity > 0)
            {
                // H 값 변화
                float hue = (Time.time * hueChangeSpeed) % 1f;
                Color color = Color.HSVToRGB(hue, saturation, value);
                foreach (var light in mirrorBallLights)
                {
                    light.color = color;
                }

                // Rotate 속도 서서히 감소
                currentRotateSpeed = Mathf.Lerp(currentRotateSpeed, 0f, Time.deltaTime);
                
                // Rotate 애니메이션
                foreach (var light in mirrorBallLights)
                {
                    light.transform.Rotate(Vector3.up * Time.deltaTime * currentRotateSpeed);
                }
                yield return null;
            }
        }

        // 현재 애니메이션 Coroutine 중지
        private void StopAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
        }
    }
}