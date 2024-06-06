using UnityEngine;
using System;
using System.Collections;


public abstract class GamePanelSubControllerBase : MonoBehaviour
{
    public UiController.STATE STATE;
    [SerializeField] protected AnimationsController animationsController;


    public virtual void SetupUI(UiController.STATE state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        /// To override
    }


    /// <summary>
    /// CLOSE ALL ANIMATED ELEMENTS OF THIS UI CONTROLLER
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator CloseAllAnimatedElements()
    {
        animationsController.Animations_ExitAll();
        animationsController.Lottie_FadeOut_All(1f);

        yield return null;

        while (animationsController.Animations_IsAnyNotInEmptyState() || animationsController._lottie_isFading)
            yield return null;
    }







    //#region TIMER

    protected float timer { get; private set; }
    private Coroutine _timerCoroutine;


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
