using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UiPlayIdleSubController : GamePanelSubControllerBase
{
    [Header("UI ELEMENTS")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private CanvasController _sessionPlayersCanvasController;
    [SerializeField] private List<Toggle> _sessionPlayersToggles;

    private PlayManager _playManager;


    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();
    }


    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        /// Don't forget to call the BASE at the init of Enter method
        base.Enter(state, runningState, callback);

        _canvasGroup.interactable = true;

        /// Create a new gameSessionData
        GameManager.gameSessionData = new Data.GameSessionData
        {
            numberOfPlayersRunning = GameManager.userData.requestedPlayers
        };

        /// Setup the  players toggle
        _sessionPlayersToggles[GameManager.userData.requestedPlayers - 1].isOn = true;

        /// Show or Hide the number of players toggle
        /// if we are in SOLO mode
        _sessionPlayersCanvasController.Toggle(GameManager.userData.requestedPlayers == 1 ? false : true);

        animationsController.Tween_PlayByName("[ENTER]");
    }


    public override IEnumerator Exit()
    {
        _canvasGroup.interactable = false;
        animationsController.Tween_PlayByName("[EXIT]");
        animationsController.Audio_PlayByName("[EXIT]");

        /// Don't forget to call the BASE at the end of Exit coroutine
        return base.Exit();
    }


    public void OnSessionPlayersNumberChanged()
    {
        if (GameManager.gameSessionData != null)
        {
            int selected = _sessionPlayersToggles.FindIndex(a => a.isOn == true);
            GameManager.gameSessionData.numberOfPlayersRunning = selected + 1;

            print("ADESSO I GIOCATORI SONO: " + GameManager.gameSessionData.numberOfPlayersRunning);
        }
    }


    /// <summary>
    /// BUTTON
    /// </summary>
    public void ButtonPressed()
    {
        animationsController.Tween_PlayByName("[CLICKED]");
        _playManager.Set_RUNNING();
    }

   

}
