using UnityEngine;
using System;
using System.Collections;

public abstract class GamePanelSubControllerBase : MonoBehaviour
{
    public UiController.STATE behaviour;

    private Coroutine _timerCoroutine;

    protected float timer { get; private set; }

    [Header("LOTTIE")]
    [SerializeField] protected LottieAnimation[] _lottieAnimations;

    public virtual void SetUI_on_STATE()
    {
        /// To override
    }
    public virtual void SetUI_on_RUNNING_STATE(UiController.RUNNING_STATE runningState, Action callback = null)
    {
        /// To override
    }



    protected void StartTimer(float seconds, Action callback = null)
    {
        if (_timerCoroutine != null) StopTimer();
        _timerCoroutine = StartCoroutine(TimerCoroutine(seconds, callback));
    }

    protected void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    private IEnumerator TimerCoroutine(float seconds, Action callback)
    {
        timer = 0f;
        while (timer < seconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (callback != null) callback.Invoke();
    }
}
