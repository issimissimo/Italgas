using UnityEngine;
using System;
using System.Collections;

public class UiController : MonoBehaviour
{
    public enum STATE { INTRO, WAITING_FOR_PLAYERS, READY_TO_START, IN_GAME, FINAL_SCORE }
    public enum RUNNING_STATE { CHAPTER, PAGE, CLICKED, WAITING }
    public STATE state { get; private set; }
    // public RUNNING_STATE runningState { get; private set; }


    [SerializeField] private GamePanelSubControllerBase[] _panels;
    private GamePanelSubControllerBase _activePanel;


    public void Set_STATE_INTRO(Action callback) => StartCoroutine(Set(newState: STATE.INTRO, callback: callback));
    public void Set_STATE_WAITING_FOR_PLAYERS() => StartCoroutine(Set(newState: STATE.WAITING_FOR_PLAYERS));
    public void Set_STATE_READY_TO_START() => StartCoroutine(Set(newState: STATE.READY_TO_START));
    public void Set_STATE_IN_GAME() => StartCoroutine(Set(newState: STATE.IN_GAME));
    public void Set_STATE_FINAL_SCORE() => StartCoroutine(Set(newState: STATE.FINAL_SCORE));
    public void Set_RUNNING_STATE_OPEN_CHAPTER(Action callback) => StartCoroutine(Set(newRunningState: RUNNING_STATE.CHAPTER, callback: callback));
    public void Set_RUNNING_STATE_OPEN_PAGE() => StartCoroutine(Set(newRunningState: RUNNING_STATE.PAGE));
    public void Set_RUNNING_STATE_ANSWER_CLICKED(Action callback) => StartCoroutine(Set(newRunningState: RUNNING_STATE.CLICKED, isNewPanel: false, callback: callback));
    public void Set_RUNNING_STATE_WAIT_OTHER_PLAYER() => StartCoroutine(Set(newRunningState: RUNNING_STATE.WAITING, isNewPanel: false));
    // public void Set_RUNNING_STATE_CLOSE_PAGE(Action callback) => StartCoroutine(Set(newRunningState: RUNNING_STATE.CLOSE_PAGE, callback: callback));


    private void Awake()
    {
        foreach (var p in _panels){
            p.siblingIndex = transform.GetSiblingIndex();
            p.GetComponent<CanvasController>().SetOff();
        } 
    }


    private void Start()
    {
        // Set_STATE_WAITING_FOR_PLAYERS();
        // Set_STATE_INTRO();

        // Set_STATE_INTRO(() => { });

        // Debug.Log("Sibling Index : " + transform.GetSiblingIndex());

    }


    private IEnumerator Set(STATE? newState = null, RUNNING_STATE? newRunningState = null, bool isNewPanel = true, Action callback = null)
    {
        if (newState != null) Debug.Log("<color=cyan>UiController - Set: </color>" + newState.ToString());
        if (newRunningState != null) Debug.Log("<color=cyan>UiController - Set: </color>" + newRunningState.ToString());

        /// This property is just used by the Managers to check the state of the UiController
        if (newState != null) state = newState.Value;


        /// We need this for the animations of Ready...        
        if (newState == STATE.IN_GAME) yield break;
        
        if (_activePanel != null && isNewPanel)
            yield return _activePanel.Exit();

        foreach (var p in _panels)
        {
            if (p.STATE == state)
            {
                p.GetComponent<CanvasController>().SetOn();
                _activePanel = p;
            }
            else p.GetComponent<CanvasController>().SetOff();
        }

        _activePanel.Enter(state, newRunningState, callback);
    }

}
