using UnityEngine;
using System;
using System.Collections;


[RequireComponent(typeof(AnimationsController))]
public abstract class GamePanelSubControllerBase : MonoBehaviour
{
    public UiController.STATE STATE;
    [SerializeField] protected AnimationsController animationsController;

    protected UiController.STATE? _currentState;
    protected UiController.RUNNING_STATE? _currentRunningState;

   

    public virtual void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        _currentState = state;
        _currentRunningState = runningState;

        string stateName = _currentRunningState != null ? _currentRunningState.ToString() : _currentState.ToString();

        print (">>>>>>>>>>>>> entro in: " + stateName.ToString());
        
        /// To be implemented
    }

    public virtual IEnumerator Exit()
    {
        /// To be implemented
        
        string stateName = _currentRunningState != null ? _currentRunningState.ToString() : _currentState.ToString();
        float exitTime = GameManager.instance.GetStateExitTime(stateName);
        print ("<<<<<<<<<<<<< esco da: " + stateName + " ---- exit time: " + exitTime);
        yield return new WaitForSeconds(exitTime);
    }


    // /// <summary>
    // /// CLOSE ALL ANIMATED ELEMENTS OF THIS UI CONTROLLER
    // /// </summary>
    // /// <returns></returns>
    // public virtual IEnumerator CloseAllAnimatedElements()
    // {
    //     /// Exit all animations
    //     yield return animationsController.Animations_ExitAll();
        
    //     /// Stop all Lottie animations
    //     animationsController.Lottie_StopAll();

    //     yield return null;
    // }







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
