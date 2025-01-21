using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace VIRDY.SDK
{
    public class VIRDYScreenRoulette : NetworkBehaviour
    {
        [SerializeField]
        private RectTransform _optionTransform;
        [SerializeField]
        private RectTransform _layoutGroup;

        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _audioClip;

        [Space]

        [SerializeField]
        private float _speed = 10f;

        [SerializeField]
        private float _optionHeight = 80f;
        [SerializeField]
        private float _spacing = 40f;

        private List<RectTransform> _options;

        private List<Transform> _refSibling;

        private Canvas _canvas;

        private int a, b, c, d, e, f;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>().rootCanvas;

            _refSibling = new List<Transform>();
            foreach (Transform transform in _layoutGroup.transform)
                _refSibling.Add(transform);

            Refresh();
        }

        private void Update() => UpdateRoulette();

        public void Refresh()
        {
            ResetSibling();
            _layoutGroup.anchoredPosition = Vector2.zero;

            _options = new List<RectTransform>();
            foreach (RectTransform option in _layoutGroup.transform)
            {
                if (option.gameObject.activeSelf)
                    _options.Add(option);
            }
        }

        public void Shuffle()
        {
            RPC_Shuffle(Random.Range(_options.Count * 5, _options.Count * 10),
                        Random.Range(_options.Count * 3, _options.Count * 5),
                        Random.Range(_options.Count * 2, _options.Count * 3),
                        Random.Range(_options.Count * 1, _options.Count * 2),
                        Random.Range(0, _options.Count * 1),
                        Random.Range(0, _options.Count));
        }

        public void UpdateRoulette()
        {
            var accel = 0f;

            if (a > 0)
                accel = 1f;
            else if (b > 0)
                accel = 0.75f;
            else if (c > 0)
                accel = 0.5f;
            else if (d > 0)
                accel = 0.25f;
            else if (e > 0)
                accel = 0.1f;
            else if (f > 0)
                accel = 0.05f;
            else
                accel = 0f;


            var translation = accel * _speed * ((_optionHeight + _spacing) * _canvas.scaleFactor) * Time.deltaTime;
            _layoutGroup.Translate(Vector3.down * translation, Space.Self);

            var distance = (_optionTransform.position.y - _layoutGroup.position.y - (_optionHeight / 2 * _canvas.scaleFactor));
            var deadline = (_optionHeight + _spacing) * _canvas.scaleFactor;
            if (distance >= deadline)
            {
                _layoutGroup.position = new Vector2(_layoutGroup.position.x, _layoutGroup.position.y + distance);
                GetLastChild()?.SetAsFirstSibling();

                if (a > 0)
                    a--;
                else if (b > 0)
                    b--;
                else if (c > 0)
                    c--;
                else if (d > 0)
                    d--;
                else if (e > 0)
                    e--;
                else if (f > 0)
                    f--;

                _audioSource.PlayOneShot(_audioClip);
            }
        }

        public void SyncActive()
        {
            for (int i = 0; i < _refSibling.Count; i++)
                RPC_SyncActive(i, _refSibling[i].gameObject.activeSelf);
        }

        private void ResetSibling()
        {
            for (int i = 0; i < _refSibling.Count; i++)
                _refSibling[i].SetAsLastSibling();
        }

        private Transform GetLastChild()
        {
            for (int i = _layoutGroup.childCount - 1; i >= 0; i--)
            {
                var child = _layoutGroup.GetChild(i);
                if (child.gameObject.activeSelf)
                    return child;
            }
            return null;
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
        private void RPC_Shuffle(int a, int b, int c, int d, int e, int f)
        {
            Refresh();

            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
        private void RPC_SyncActive(int index, bool active)
        {
            _refSibling[index].gameObject.SetActive(active);
        }
    }
}
