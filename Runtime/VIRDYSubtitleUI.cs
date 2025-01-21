using UnityEngine;
using UnityEngine.Events;
using TMPro;
#if VIRDY_DOTWEEN
using DG.Tweening;
#endif
using System.Collections;

namespace VIRDY.SDK
{
    [RequireComponent(typeof(CanvasGroup))]
    public class VIRDYSubtitleUI : VIRDYBehaviour
    {
        public static UnityAction<string> SetSubtitleText;
        public static UnityAction ShowSubtitle;
        public static UnityAction HideSubtitle;

        [SerializeField]
        private TextMeshProUGUI _subtitleText;

        [SerializeField]
        private Vector2 _anchorPositionWhenVisible;

        [SerializeField]
        private Vector2 _anchorPositionWhenHidden;

        private RectTransform _rectTransform;

        private CanvasGroup _canvasGroup;

#if VIRDY_DOTWEEN
        [SerializeField]
        private float _duration = 1f;

        [SerializeField]
        private Ease _inEase = Ease.OutSine;

        [SerializeField]
        private Ease _outEase = Ease.InSine;
#endif

        [SerializeField]
        private bool _enableTypewriterEffect = false;
        
        [SerializeField]
        private float _typewriterSpeed = 0.05f; // Duration between characters

        protected override void OnInitialize()
        {
            _rectTransform = this.GetComponent<RectTransform>();
            _canvasGroup = this.GetComponent<CanvasGroup>();

            _rectTransform.anchoredPosition = _anchorPositionWhenHidden;
            _canvasGroup.alpha = 0f;

            SetSubtitleText += SetText;
            ShowSubtitle += Show;
            HideSubtitle += Hide;
        }

        public void SetText(string text)
        {
            StopAllCoroutines();
            _subtitleText.text = "";
            if (_enableTypewriterEffect)
            {
                StartCoroutine(TypeText(text));
            }
            else
            {
                _subtitleText.text = text;
            }
        }

        private IEnumerator TypeText(string text)
        {
            _subtitleText.text = "";
            foreach (char letter in text)
            {
                _subtitleText.text += letter;
                yield return new WaitForSeconds(_typewriterSpeed);
            }
        }

        public void Show() 
        {
#if VIRDY_DOTWEEN
            DOTween.Kill(_rectTransform);
            DOTween.Kill(_canvasGroup);
            _rectTransform.DOAnchorPos(_anchorPositionWhenVisible, _duration).SetEase(_inEase);
            _canvasGroup.DOFade(1f, _duration).SetEase(_inEase);
#endif
        }

        public void Hide()
        {
#if VIRDY_DOTWEEN
            DOTween.Kill(_rectTransform);
            DOTween.Kill(_canvasGroup);
            _rectTransform.DOAnchorPos(_anchorPositionWhenHidden, _duration).SetEase(_outEase);
            _canvasGroup.DOFade(0f, _duration).SetEase(_outEase);
#endif
        }

        public void EnableTypewriterEffect(bool enable)
        {
            _enableTypewriterEffect = enable;
        }
    }
}
