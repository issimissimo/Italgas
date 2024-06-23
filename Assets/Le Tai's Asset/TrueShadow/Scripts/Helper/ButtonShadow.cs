using System.Security.AccessControl;
using System;
using UnityEngine;
using System.Collections;


namespace LeTai.TrueShadow
{
    [RequireComponent(typeof(TrueShadow))]

    public class ButtonShadow : AnimatedBiStateButton
    {
        [SerializeField] float _clickedShadowSize = 8f;
        [SerializeField] float _pressAnimationTime = 0.1f;
        [SerializeField] float _releaseAnimationTime = 0.1f;
        [SerializeField] bool _glowShadow;
        private float _defaultSize;
        private UnityEngine.Color _defaultColor;
        private TrueShadow _shadow;


        void Awake()
        {
            _shadow = GetComponent<TrueShadow>();
            _defaultSize = _shadow.Size;
            _defaultColor = _shadow.Color;
        }

        void OnEnable()
        {
            SetDefault();
        }

        protected override void OnWillPress()
        {
            var newColor = _glowShadow ? UnityEngine.Color.white : _defaultColor;
            StartCoroutine(AnimateCoroutine(_defaultSize, _clickedShadowSize, _defaultColor, newColor, _pressAnimationTime, ()=>
            {
                StartCoroutine(AnimateCoroutine(_clickedShadowSize, _defaultSize, newColor, newColor, _releaseAnimationTime));
            }));

            base.OnWillPress();
        }

        public void SetDefault()
        {
            _shadow.Size = _defaultSize;
            _shadow.Color = _defaultColor;
        }


        private IEnumerator AnimateCoroutine(float initSize, float endSize, UnityEngine.Color initColor, UnityEngine.Color endColor, float time, Action onEnd = null)
        {
            float t = 0f;
            while (t <= time)
            {
                t += Time.deltaTime;
                _shadow.Size = Mathf.Lerp(initSize, endSize, t / time);
                _shadow.Color = UnityEngine.Color.Lerp(initColor, endColor, t / time);
                yield return null;
            }
            onEnd?.Invoke();
        }


    }
}
