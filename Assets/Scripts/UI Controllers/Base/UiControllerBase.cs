using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public abstract class UiControllerBase : MonoBehaviour
{
    public enum STATE { WAITING_FOR_PLAYERS, READY_TO_START, IN_GAME }
    public enum RUNNING_STATE { OPEN_CHAPTER, OPEN_PAGE, WAIT_OTHER_PLAYER, CLOSE_PAGE, FINAL_SCORE }
    public STATE state { get; protected set; }
    public RUNNING_STATE runningState { get; protected set; }

    public static event Action<STATE> UpdateAnimationsOnStateChange; /// Event to subscribe, to update the UI animations
    public static event Action<RUNNING_STATE> UpdateAnimationsOnRunningStateChange; /// Event to subscribe, to update the UI animations

    [SerializeField] protected List<CanvasController> _uiPanels;



    public void Set_STATE_WAITING_FOR_PLAYERS() => StartCoroutine(SetStateCoroutine(STATE.WAITING_FOR_PLAYERS));
    public void Set_STATE_READY_TO_START() => StartCoroutine(SetStateCoroutine(STATE.READY_TO_START));
    public void Set_STATE_IN_GAME() => StartCoroutine(SetStateCoroutine(STATE.IN_GAME));



    public void Set_RUNNING_STATE_OPEN_CHAPTER() => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.OPEN_CHAPTER));
    public void Set_RUNNING_STATE_OPEN_PAGE() => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.OPEN_PAGE));
    public void Set_RUNNING_STATE_WAIT_OTHER_PLAYER() => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.WAIT_OTHER_PLAYER));
    public void Set_RUNNING_STATE_CLOSE_PAGE(Action callback) => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.CLOSE_PAGE, closeAnimations: false, callback));
    public void Set_RUNNING_STATE_FINAL_SCORE() => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.FINAL_SCORE));



    protected virtual void StateChanged()
    {
        /// to override
    }

    protected virtual async void RunningStateChanged()
    {
        /// to override
    }


    private IEnumerator SetStateCoroutine(STATE newState)
    {
        /// EXIT all animations before to proceed to new state
        yield return AnimationsManager.instance.ExitAllAnimationsAndWaitForFinish();

        /// Set new state
        state = newState;

        /// Call the Method of the Classes that derive from this one
        StateChanged();

        Set_PanelsUI_on_STATE();

        /// ENTER new animations
        if (UpdateAnimationsOnStateChange != null) UpdateAnimationsOnStateChange(state);
    }


    private IEnumerator SetRunningStateCoroutine(RUNNING_STATE newRunningState, bool closeAnimations = true, Action callback = null)
    {
        if (closeAnimations)
        {
            /// EXIT all animations that are not in EMPTY state before to proceed to new state
            yield return AnimationsManager.instance.ExitAllAnimationsAndWaitForFinish();
        }
        else yield return null;

        /// Set new running state
        runningState = newRunningState;

        /// Setup the UI of the new panel
        Set_PanelsUI_on_RUNNING_STATE(callback);

        /// Call the Method of the Classes that derive from this one
        RunningStateChanged();

        /// ENTER new animations
        if (UpdateAnimationsOnRunningStateChange != null) UpdateAnimationsOnRunningStateChange(runningState);
    }


    protected void Set_PanelsUI_on_STATE()
    {
        foreach (var panel in _uiPanels)
        {
            var panelController = panel.gameObject.GetComponent<GamePanelSubControllerBase>();

            if (panelController.behaviour == state)
            {
                panel.SetOn();
                panelController.SetUI_on_STATE();
            }
            else
            {
                panel.SetOff();
            }
        }
    }

    protected void Set_PanelsUI_on_RUNNING_STATE(Action callback = null)
    {
        foreach (var panel in _uiPanels)
        {
            var panelController = panel.GetComponent<GamePanelSubControllerBase>();

            if (panelController.behaviour == state)
            {
                panelController.SetUI_on_RUNNING_STATE(runningState, callback);
            }
        }
    }


    // /// <summary>
    // /// Check if ANY "UiAnimatedElement" is NOT in EMPTY state
    // /// </summary>
    // /// <param name="animations"></param>
    // /// <returns></returns> <summary>
    // private bool IsAnyAnimationPlaying(UiAnimatedElement[] animations)
    // {
    //     var firstMatch = Array.Find(animations, elem => elem.IsOnEmptyState() == false);
    //     return firstMatch == null ? false : true;
    // }
}
