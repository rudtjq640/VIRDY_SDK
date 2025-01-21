#if VIRDY_DOTWEEN
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace VIRDY.SDK
{
    public static class VIRDYDOTweenModule
    {
        public static TweenerCore<float, float, FloatOptions> DOFade(this CanvasGroup target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.alpha, x => target.alpha = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPos(this RectTransform target, Vector2 endValue, float duration, bool snapping = false)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, endValue, duration);
            t.SetOptions(snapping).SetTarget(target);
            return t;
        }
    }
}
#endif
