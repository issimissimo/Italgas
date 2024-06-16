using UnityEngine;

namespace UnityExtensions.Tween.Sample
{
    public class UIAnimatedState : BaseStackStateComponent
    {
        public CanvasGroup canvasGroup;
        public TweenPlayer showingAnimation;
        public TweenPlayer suspendingAnimation;

        void Awake()
        {
            showingAnimation.onForwardArrived += () => canvasGroup.interactable = true;
            showingAnimation.onBackArrived += () => gameObject.SetActive(false);

            suspendingAnimation.onBackArrived += () =>
            {
                canvasGroup.interactable = true;
                suspendingAnimation.gameObject.SetActive(false);
            };
        }

        public override void OnReset()
        {
            showingAnimation.normalizedTime = 0f;
            showingAnimation.enabled = false;

            suspendingAnimation.normalizedTime = 0f;
            suspendingAnimation.enabled = false;

            gameObject.SetActive(false);
            suspendingAnimation.gameObject.SetActive(false);
        }

        public override void OnPush()
        {
            canvasGroup.interactable = false;
            gameObject.SetActive(true);
            showingAnimation.SetForwardDirectionAndEnabled();                                                                                                                                                                                   
        }

        public override void OnPop()
        {
            canvasGroup.interactable = false;
            showingAnimation.SetBackDirectionAndEnabled();                                                                                                                                                                                   
        }

        public override void OnSuspend()
        {
            canvasGroup.interactable = false;
            suspendingAnimation.gameObject.SetActive(true);
            suspendingAnimation.SetForwardDirectionAndEnabled();
        }

        public override void OnResume()
        {
            suspendingAnimation.SetBackDirectionAndEnabled();
        }
    }
}