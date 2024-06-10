using UnityEngine;
using System;
using System.Collections;

public class UiController : MonoBehaviour
{
    public enum STATE { INTRO, WAITING_FOR_PLAYERS, READY_TO_START, IN_GAME, FINAL_SCORE }
    public enum RUNNING_STATE { OPEN_CHAPTER, OPEN_PAGE, ANSWER_CLICKED, WAIT_OTHER_PLAYER, CLOSE_PAGE }
    public STATE state { get; private set; }
    public RUNNING_STATE runningState { get; private set; }
    // public int uiPlayerId = 999;

    [SerializeField] private GamePanelSubControllerBase[] _panels;
    private GamePanelSubControllerBase _activePanel;


    public void Set_STATE_INTRO(Action callback) => StartCoroutine(Set(newState: STATE.INTRO, closeAnimations: false, callback: callback));
    public void Set_STATE_WAITING_FOR_PLAYERS() => StartCoroutine(Set(newState: STATE.WAITING_FOR_PLAYERS));
    public void Set_STATE_READY_TO_START() => StartCoroutine(Set(newState: STATE.READY_TO_START));
    public void Set_STATE_IN_GAME() => StartCoroutine(Set(newState: STATE.IN_GAME));
    public void Set_STATE_FINAL_SCORE() => StartCoroutine(Set(newState: STATE.FINAL_SCORE));
    public void Set_RUNNING_STATE_OPEN_CHAPTER(Action callback) => StartCoroutine(Set(newRunningState: RUNNING_STATE.OPEN_CHAPTER, callback: callback));
    public void Set_RUNNING_STATE_OPEN_PAGE() => StartCoroutine(Set(newRunningState: RUNNING_STATE.OPEN_PAGE));
    public void Set_RUNNING_STATE_ANSWER_CLICKED(Action callback) => StartCoroutine(Set(newRunningState: RUNNING_STATE.ANSWER_CLICKED, closeAnimations: false, callback: callback));
    public void Set_RUNNING_STATE_WAIT_OTHER_PLAYER() => StartCoroutine(Set(newRunningState: RUNNING_STATE.WAIT_OTHER_PLAYER, closeAnimations: false));
    public void Set_RUNNING_STATE_CLOSE_PAGE(Action callback) => StartCoroutine(Set(newRunningState: RUNNING_STATE.CLOSE_PAGE, closeAnimations: false, callback: callback));


    private void Awake()
    {
        foreach(var p in _panels) p.GetComponent<CanvasController>().SetOff();
    }


    private void Start()
    {
        // Set_STATE_WAITING_FOR_PLAYERS();
        // Set_STATE_INTRO();

        // Set_STATE_INTRO(() => { });
    }


    private IEnumerator Set(STATE? newState = null, RUNNING_STATE? newRunningState = null, bool closeAnimations = true, Action callback = null)
    {
        if (newState != null) print(gameObject.name +  " - Set STATE --> " + newState.ToString());
        if (newRunningState != null) print(gameObject.name +  " - Set RUNNING_STATE --> " + newRunningState.ToString());

        if (newState != null) state = newState.Value;
        if (newRunningState != null) runningState = newRunningState.Value;


        if (_activePanel != null && closeAnimations)
        {
            yield return _activePanel.CloseAllAnimatedElements();
        }

        foreach (var p in _panels)
        {
            if (p.STATE == state)
            {
                p.GetComponent<CanvasController>().SetOn();
                _activePanel = p;
            }
            else p.GetComponent<CanvasController>().SetOff();
        }

        _activePanel.SetupUI(state, newRunningState, callback);
    }
}
