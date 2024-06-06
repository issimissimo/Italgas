using UnityEngine;
using TMPro;
using System;

public class UiPlayWaitingSubController : GamePanelSubControllerBase
{
    [Header("UI ELEMENTS")]
    [SerializeField] private TMP_Text _message;

    private PlayManager _playManager;

    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();
    }

    public override void SetupUI(UiController.STATE state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        if (GameManager.userData.requestedPlayers == 1)
        {
            GameManager.instance.ShowNotification("Sei in modalità SOLO, quindi non possono collegarsi altri giocatori");

            _message.text = "Attendi un attimo...";
        }
        else
        {
            if (_playManager.otherPlayer != null)
            {
                if (_playManager.otherPlayer.NetworkedSessionRequestedPlayers == 1)
                {
                    _message.text = "L'altro Player gioca da solo! Attendi...";
                }
            }
            else
            {
                int otherTabletNumber = GameManager.userData.playerId == 0 ? 2 : 1;
                _message.text = "In attesa di collegamento con il tablet " + otherTabletNumber.ToString();
            }
        }

        animationsController.Animations_EnterAll();
        animationsController.Lottie_FadeIn_All(time: 1f);
    }
}
