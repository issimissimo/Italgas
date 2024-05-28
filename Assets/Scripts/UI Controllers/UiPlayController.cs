using System.Threading.Tasks;

public class UiPlayController : UiControllerBase
{
    private void Start()
    {
        Set_STATE_WAITING_FOR_PLAYERS();
    }


    protected override void StateChanged()
    {
        switch (state)
        {
            case STATE.WAITING_FOR_PLAYERS:

                if (GameManager.userData.requestedPlayers == 1)
                {
                    GameManager.instance.ShowNotification("Sei in modalità SOLO, quindi non possono collegarsi altri giocatori");
                }
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

        // Set_PanelsUI_on_STATE();
    }


    protected override async void RunningStateChanged()
    {
        // Set_PanelsUI_on_RUNNING_STATE();

        switch (runningState)
        {
            case RUNNING_STATE.NEW_CHAPTER:
                
                await Task.Delay(3000);
                Set_RUNNING_STATE_NEW_PAGE();
                break;

            case RUNNING_STATE.NEW_PAGE:

                GameManager.instance.StartTimer(
                    seconds: GameManager.instance.maximumSeconds,
                    callback: () =>
                    {
                        print("TEMPO SCADUTO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        var manager = FindObjectOfType<PlayManager>();
                        manager.OnAnswerButtonPressed(false);
                    }
                );
                break;

            case RUNNING_STATE.FINAL_SCORE:
                break;
        }

        // Set_PanelsUI_on_RUNNING_STATE();
    }
}
