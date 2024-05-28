using System.Collections.Generic;
using Fusion;
using System.Threading.Tasks;

public class PlayerController : NetworkBehaviour
{
    public enum STATE { IDLE, RUNNING }
    public enum RUNNING_STATE { NONE, THINKING, FINISHED }


    //#region GENERAL
    [Networked]
    public int NetworkedPlayerId { get; set; }
    [Networked]
    public int NetworkedSessionRequestedPlayers { get; set; }
    //#endregion

    //#region SCORE
    [Networked]
    public bool NetworkedAnswerResult { get; set; }
    [Networked]
    public float NetworkedTimeSpent { get; set; }
    //#endregion

    //#region STATES
    [Networked, OnChangedRender(nameof(OnStateChanged))]
    public STATE NetworkedState { get; set; }

    [Networked, OnChangedRender(nameof(OnRunningStateChanged))]
    public RUNNING_STATE NetworkedRunningState { get; set; }
    //#endregion



    private NetworkManagerBase _networkManager;


    private void Awake()
    {
        /// Retrieve the networkManager of the scene
        _networkManager = FindObjectOfType<NetworkManagerBase>();
    }

    private async void Start()
    {
        if (HasStateAuthority)
        {
            NetworkedPlayerId = GameManager.userData.playerId;
            NetworkedState = STATE.IDLE;
            NetworkedRunningState = RUNNING_STATE.NONE;
            NetworkedSessionRequestedPlayers = GameManager.userData.requestedPlayers;
        }

        /// Let's wait a little, to try to solve the ID Player bug...
        await Task.Delay(500);

        _networkManager.OnPlayersCountChanged();
    }

    private void OnStateChanged()
    {
        _networkManager.OnPlayerStateChanged(NetworkedPlayerId, NetworkedState);
    }

    private void OnRunningStateChanged()
    {
        /// ADD HERE THE SCORE OF THE PLAYER
        if (NetworkedRunningState == RUNNING_STATE.FINISHED)
        {
            /// Initialize
            if (GameManager.gameSessionData.scores[NetworkedPlayerId] == null)
            {
                GameManager.gameSessionData.scores[NetworkedPlayerId] = new Data.TotalPlayerScore
                {
                    singlePlayerScoreList = new List<Data.SinglePlayerScore>()
                };
            }

            /// Add the score
            GameManager.gameSessionData.scores[NetworkedPlayerId].singlePlayerScoreList.Add(
                new Data.SinglePlayerScore
                {
                    isCorrect = NetworkedAnswerResult,
                    timeSpent = NetworkedTimeSpent
                });


            print("******** SCORE PLAYER " + NetworkedPlayerId + " **********");
            print("Tempo: " + NetworkedTimeSpent);
            print("Risposta giusta? " + NetworkedAnswerResult);
            print("*****************");
        }

        /// Callback
        _networkManager.OnPlayerRunningStateChanged(NetworkedPlayerId, NetworkedRunningState);
    }



    /// <summary>
    /// Send / Receive the message to restart
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SendMessageToRestart(string message, RpcInfo info = default)
    {
        if (!info.IsInvokeLocal)
        {
            print("I'VE RECEIVED THE MESSAGE THAT I MUST RESTART");
            GameManager.instance.Restart();
        }
    }


    public async void SendMessageToRestart()
    {
        /// we must wait some time (how much??) because immediately doesn't work
        await Task.Delay(1000);
        RPC_SendMessageToRestart("");
    }



    /// <summary>
    /// Set STATE
    /// </summary>

    public void Set_STATE_IDLE() => NetworkedState = STATE.IDLE;

    public void Set_STATE_RUNNING(int? runningPlayersNumber = null)
    {
        NetworkedSessionRequestedPlayers = runningPlayersNumber != null ? runningPlayersNumber.Value : GameManager.gameSessionData.numberOfPlayersRunning;
        NetworkedState = STATE.RUNNING;
    }


    /// <summary>
    /// Set RUNNING_STATE
    /// </summary>

    public void Set_RUNNING_STATE_NONE() => NetworkedRunningState = RUNNING_STATE.NONE;

    public void Set_RUNNING_STATE_THINKING() => NetworkedRunningState = RUNNING_STATE.THINKING;

    public void Set_RUNNING_STATE_FINISHED(bool result)
    {
        NetworkedTimeSpent = GameManager.timer;
        NetworkedAnswerResult = result;
        NetworkedRunningState = RUNNING_STATE.FINISHED;
    }


    // private ChangeDetector _changeDetector;

    // public override void Spawned()
    // {
    //     print("+++++++++++++++++ Spawned");
    //     _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    // }

    // public override void FixedUpdateNetwork()
    // {
    //     foreach (var change in _changeDetector.DetectChanges(this))
    //     {
    //         switch (change)
    //         {
    //             case nameof(NetworkedState):
    //             print("+++++++++ Cambiato STATE: " + NetworkedState.ToString());
    //             OnStateChanged();
    //             break;

    //             case nameof(NetworkedRunningState):
    //             print("+++++++++ Cambiato RUNNING_STATE: " + NetworkedRunningState.ToString());
    //             OnRunningStateChanged();
    //             break;
    //         }
    //     }
    // }
}
