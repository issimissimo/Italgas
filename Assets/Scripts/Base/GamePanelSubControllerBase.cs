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
    public int siblingIndex {get; set;} /// We use this index to identify the Player ID on the Viewer version


    public virtual void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        _currentState = state;
        _currentRunningState = runningState;

        string stateName = _currentRunningState != null ? _currentRunningState.ToString() : _currentState.ToString();
        Debug.Log("<color=blue>>>>>>>>>>>>>> </color> entro in: " + stateName.ToString() + " ID:" + siblingIndex);
        
        /// To be implemented
    }

    public virtual IEnumerator Exit()
    {
        /// To be implemented
        
        string stateName = _currentRunningState != null ? _currentRunningState.ToString() : _currentState.ToString();
        float exitTime = GameManager.instance.GetStateExitTime(stateName);
        Debug.Log("<color=blue><<<<<<<<<<<<< </color> esco da: " + stateName.ToString() + " ---- exit time: " + exitTime + " ID:" + siblingIndex);

        yield return new WaitForSeconds(exitTime);
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
