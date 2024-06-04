using UnityEngine;

public class UiPlayController : UiControllerBase
{
    // [SerializeField] LottieAnimation[] _lottieAnimations;
    
    private void Start()
    {
        Set_STATE_WAITING_FOR_PLAYERS();
    }



    /// <summary>
    /// THIS METHOD IS CALLED AFTER "Set_PanelsUI_on_STATE"
    /// OF THE BASE CLASS
    /// </summary>
    /// <returns></returns>
    protected override void StateChanged()
    {
        switch (state)
        {
            case STATE.WAITING_FOR_PLAYERS:

                // if (GameManager.userData.requestedPlayers == 1)
                // {
                //     GameManager.instance.ShowNotification("Sei in modalità SOLO, quindi non possono collegarsi altri giocatori");
                // }

                // Lottie.instance.PlayByName("WaitingSmile", _lottieAnimations);

                break;

            case STATE.READY_TO_START:

                GameManager.gameSessionData = new Data.GameSessionData
                {
                    numberOfPlayersRunning = GameManager.userData.requestedPlayers
                };
                break;

            case STATE.IN_GAME:
                break;
        }
    }

}
