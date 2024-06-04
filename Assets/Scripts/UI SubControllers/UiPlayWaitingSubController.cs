using UnityEngine;
using TMPro;

public class UiPlayWaitingSubController : GamePanelSubControllerBase
{
    [SerializeField] private TMP_Text _message;

    public override void SetUI_on_STATE()
    {
        if (GameManager.userData.requestedPlayers == 1)
        {
            /// Show Notification for SOLO mode
            GameManager.instance.ShowNotification("Sei in modalità SOLO, quindi non possono collegarsi altri giocatori");

            _message.text = "Attendi un attimo...";
        }
        else
        {
            int otherTabletNumber = GameManager.userData.playerId == 0 ? 2 : 1;
            _message.text = "In attesa di collegamento con il tablet " + otherTabletNumber.ToString();
        }

        /// Play Lottie animation
        // Lottie.instance.PlayByName("WaitingSmile", _lottieAnimations);
        Lottie.instance.FadeIn(_lottieAnimations);
    }
}
