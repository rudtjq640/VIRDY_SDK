using UnityEngine;
using UnityEngine.UI;
#if VIRDY_DOTWEEN
using DG.Tweening;
#endif

namespace VIRDY.SDK
{
    [RequireComponent(typeof(RectTransform))]
    public class VIRDYFadeUI : VIRDYBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private bool _movePosition;

        [SerializeField]
        private Vector2 _anchorPositionWhenVisible;

        [SerializeField]
        private Vector2 _anchorPositionWhenHidden;

#if VIRDY_DOTWEEN
        [SerializeField]
        private float _duration = 1f;

        [SerializeField]
        private Ease _inEase = Ease.OutSine;

        [SerializeField]
        private Ease _outEase = Ease.InSine;
#endif

        private void Awake()
        {
            EnsureComponents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureComponents();
        }
#endif

        private void EnsureComponents()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        public void FadeIn()
        {
#if VIRDY_DOTWEEN
            DOTween.Kill(_canvasGroup);
            _canvasGroup.DOFade(1f, _duration).SetEase(_inEase);
            if (_movePosition == true)
            {
                DOTween.Kill(_rectTransform);
                _rectTransform.DOAnchorPos(_anchorPositionWhenVisible, _duration).SetEase(_inEase);
            }
#endif
        }

        public void FadeOut()
        {
#if VIRDY_DOTWEEN
            DOTween.Kill(_canvasGroup);
            _canvasGroup.DOFade(0f, _duration).SetEase(_outEase);
            if (_movePosition == true)
            {
                DOTween.Kill(_rectTransform);
                _rectTransform.DOAnchorPos(_anchorPositionWhenHidden, _duration).SetEase(_outEase);
            }
#endif
        }
    }
}
