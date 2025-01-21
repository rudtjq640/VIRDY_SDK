using UnityEngine;

namespace VIRDY.SDK
{
    public class VIRDYPresentation : MonoBehaviour
    {
        public GameObject RootObject;
        [SerializeField] private GameObject[] _slides;
        private int _currentIndex = 0;

        private void Start()
        {
            if (RootObject != null)
            {
                AutoFillSlides();
            }
            UpdateSlidesVisibility();
        }

        private void UpdateSlidesVisibility()
        {
            for (int i = 0; i < _slides.Length; i++)
            {
                _slides[i].SetActive(i == _currentIndex);
            }
        }

        public void NextSlide()
        {
            if (_currentIndex < _slides.Length - 1)
            {
                _currentIndex++;
                UpdateSlidesVisibility();
            }
        }

        public void PreviousSlide()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                UpdateSlidesVisibility();
            }
        }

        public void AutoFillSlides()
        {
            if (RootObject != null)
            {
                var spriteRenderers = RootObject.GetComponentsInChildren<SpriteRenderer>();
                _slides = new GameObject[spriteRenderers.Length];
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    _slides[i] = spriteRenderers[i].gameObject;
                }
            }
        }
    }
}