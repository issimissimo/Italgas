using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class UiController : MonoBehaviour
{
    public enum STATE { WAITING_FOR_PLAYERS, READY_TO_START, IN_GAME }
    public enum RUNNING_STATE { OPEN_CHAPTER, OPEN_PAGE, ANSWER_CLICKED, WAIT_OTHER_PLAYER, CLOSE_PAGE, FINAL_SCORE }
    public STATE state { get; private set; }
    public RUNNING_STATE runningState { get; private set; }

    public static event Action<STATE> UpdateAnimationsOnStateChange; /// Event to subscribe, to update the UI animations
    public static event Action<RUNNING_STATE> UpdateAnimationsOnRunningStateChange; /// Event to subscribe, to update the UI animations

    [SerializeField] private List<CanvasController> _uiPanels;



    public enum DOMAIN { STATE, RUNNING_STATE }
    private GamePanelSubControllerBase _oldPanel;



    public void Set_STATE_WAITING_FOR_PLAYERS() => StartCoroutine(SetStateCoroutine(STATE.WAITING_FOR_PLAYERS));
    public void Set_STATE_READY_TO_START() => StartCoroutine(SetStateCoroutine(STATE.READY_TO_START));
    public void Set_STATE_IN_GAME() => StartCoroutine(SetStateCoroutine(STATE.IN_GAME));



    public void Set_RUNNING_STATE_OPEN_CHAPTER(Action callback) => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.OPEN_CHAPTER, callback: callback));
    public void Set_RUNNING_STATE_OPEN_PAGE() => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.OPEN_PAGE));
    public void Set_RUNNING_STATE_ANSWER_CLICKED(Action callback) => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.ANSWER_CLICKED, closeAnimations: false, callback: callback));
    public void Set_RUNNING_STATE_WAIT_OTHER_PLAYER() => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.WAIT_OTHER_PLAYER, closeAnimations: false));
    public void Set_RUNNING_STATE_CLOSE_PAGE(Action callback) => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.CLOSE_PAGE, closeAnimations: false, callback: callback));
    public void Set_RUNNING_STATE_FINAL_SCORE() => StartCoroutine(SetRunningStateCoroutine(RUNNING_STATE.FINAL_SCORE));


    private void Start()
    {
        Set_STATE_WAITING_FOR_PLAYERS();
    }

    // protected virtual void StateChanged()
    // {
    //     /// to override
    // }


    private IEnumerator SetStateCoroutine(STATE newState)
    {
        /// Set new state
        state = newState;
        print("========> STATE: " + state.ToString());

        /// EXIT all animations before to proceed to new state
        yield return Animations.instance.ExitAllAnimationsAndWaitForFinish();

        /// STOP all LOTTIE animations before to proceed to new state
        // yield return StopAllLottieAnimations();


        Set_PanelsUI_on_STATE();

        /// ENTER new animations
        if (UpdateAnimationsOnStateChange != null) UpdateAnimationsOnStateChange(state);
    }


    private IEnumerator SetRunningStateCoroutine(RUNNING_STATE newRunningState, bool closeAnimations = true, Action callback = null)
    {
        /// Set new running state
        runningState = newRunningState;
        print("========> RUNNING_STATE: " + runningState.ToString());

        if (closeAnimations)
        {
            /// EXIT all animations that are not in EMPTY state before to proceed to new state
            yield return Animations.instance.ExitAllAnimationsAndWaitForFinish();

            /// STOP all LOTTIE animations before to proceed to new state
            // yield return StopAllLottieAnimations();
            // yield return Lottie.instance.Stop_All_Coroutine();

        }


        /// Setup the UI of the new panel
        Set_PanelsUI_on_RUNNING_STATE(callback);


        /// ENTER new animations
        if (UpdateAnimationsOnRunningStateChange != null) UpdateAnimationsOnRunningStateChange(runningState);
    }


    private void Set_PanelsUI_on_STATE()
    {
        foreach (var panel in _uiPanels)
        {
            var panelController = panel.gameObject.GetComponent<GamePanelSubControllerBase>();

            if (panelController.STATE == state)
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

    private void Set_PanelsUI_on_RUNNING_STATE(Action callback = null)
    {
        foreach (var panel in _uiPanels)
        {
            var panelController = panel.GetComponent<GamePanelSubControllerBase>();

            if (panelController.STATE == state)
                panelController.SetUI_on_RUNNING_STATE(runningState, callback);
        }
    }


}
