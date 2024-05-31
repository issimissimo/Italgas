using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayIdleSubController : GamePanelSubControllerBase
{
    [SerializeField] private List<Toggle> _sessionPlayersToggles;
    [SerializeField] private CanvasController _sessionPlayersCanvasController;
    [SerializeField] private UiAnimatedElementBase _titlesAnimation;
    [SerializeField] private RiveAsset _riveAsset;
   


    public override void SetUI_on_STATE()
    {
        _sessionPlayersToggles[GameManager.userData.requestedPlayers - 1].isOn = true;

        for (int i = 0; i < _sessionPlayersToggles.Count; i++)
        {
            if (i == GameManager.gameSessionData.numberOfPlayersRunning - 1)
                _sessionPlayersToggles[i].isOn = true;
        }

        _sessionPlayersCanvasController.Toggle(GameManager.userData.requestedPlayers == 1 ? false : true);

        // /// Actually I don't have a better way to manage the ENTER state
        // /// of this particular animation...
        // _titlesAnimation.Enter();
        _riveAsset.StartRiveAsset();
    }



    public void OnSessionPlayersNumberChanged()
    {
        if (GameManager.gameSessionData != null)
        {
            int selected = _sessionPlayersToggles.FindIndex(a => a.isOn == true);
            GameManager.gameSessionData.numberOfPlayersRunning = selected + 1;
        }
    }


}
