using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiPlayIdleSubController : GamePanelSubControllerBase
{
    [Header("UI ELEMENTS")]
    [SerializeField] private CanvasController _sessionPlayersCanvasController;
    [SerializeField] private List<Toggle> _sessionPlayersToggles;
    


    public override void SetupUI(UiController.STATE state, UiController.RUNNING_STATE? runningState, Action callback)
    {
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

        Animations_EnterAll();
        Lottie_FadeIn_All(time: 1f);
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


}
