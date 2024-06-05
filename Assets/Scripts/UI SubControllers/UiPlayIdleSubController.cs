using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayIdleSubController : GamePanelSubControllerBase
{
    [SerializeField] private List<Toggle> _sessionPlayersToggles;
    [SerializeField] private CanvasController _sessionPlayersCanvasController;


    public override void SetUI_on_STATE()
    {
        /// Create a new gameSessionData
        GameManager.gameSessionData = new Data.GameSessionData
        {
            numberOfPlayersRunning = GameManager.userData.requestedPlayers
        };

        /// Setup the  players toggle
        _sessionPlayersToggles[GameManager.userData.requestedPlayers - 1].isOn = true;

        // for (int i = 0; i < _sessionPlayersToggles.Count; i++)
        // {
        //     if (i == GameManager.gameSessionData.numberOfPlayersRunning - 1)
        //         _sessionPlayersToggles[i].isOn = true;
        // }

        /// Show or Hide the number of players toggle
        /// if we are in SOLO mode
        _sessionPlayersCanvasController.Toggle(GameManager.userData.requestedPlayers == 1 ? false : true);

        /// Play Lottie animation
        Lottie.instance.FadeIn(_loopLottieAnimations, 2f);
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
