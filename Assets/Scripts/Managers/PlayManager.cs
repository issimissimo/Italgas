using System.Threading.Tasks;
using UnityEngine;

public class PlayManager : NetworkManagerBase
{
    // [SerializeField] private UiPlayController _uiController;
    // private PlayerController _myPlayer = null;
    private PlayerController _otherPlayer = null;







    //#region INTERACTIONS FROM UI

    /// <summary>
    /// CLICKED ON THE ANSWER IN THE UI
    /// </summary>
    /// <param name="buttonNumber"></param> <summary>
    public void OnAnswerButtonPressed(int buttonNumber, bool isTrue)
    {
        _myPlayer.Set_RUNNING_STATE_FINISHED(buttonNumber, isTrue);
    }



    /// <summary>
    /// BUTTON
    /// </summary>
    public void Set_RUNNING()
    {
        _myPlayer.Set_STATE_RUNNING();
    }

    /// <summary>
    /// BUTTON
    /// </summary>
    public void Set_IDLE()
    {
        _myPlayer.Set_STATE_IDLE();
    }



    //#region RUNNING GAME LOGICS

    /// <summary>
    /// 
    /// </summary>
    // private void ProceedToNext()
    // {
    //     if (GameManager.currentGamePageIndex <= GameManager.currentGameChapter.pages.Count - 1)
    //     {
    //         if (GameManager.currentGamePageIndex == 0) _uiController.Set_RUNNING_STATE_NEW_CHAPTER();
    //         else _uiController.Set_RUNNING_STATE_NEW_PAGE();
    //     }
    //     else
    //     {
    //         if (GameManager.currentGameChapterIndex < GameManager.currentGameVersion.chapters.Count - 1)
    //         {
    //             GameManager.currentGameChapterIndex++;
    //             GameManager.currentGamePageIndex = 0;

    //             ProceedToNext();
    //         }
    //         else
    //         {
    //             print ("------------ FINITO!!!!!!!!!!!");

    //             /// Game is finished!!!
    //             GameManager.currentGameChapterIndex = 0;
    //             GameManager.currentGamePageIndex = -1;

    //             /// Set my Player
    //             // _myPlayer.NetworkedState = PlayerController.STATE.FINISHED;
    //             _myPlayer.Set_RUNNING_STATE_NONE();

    //             /// UI
    //             _uiController.Set_RUNNING_STATE_FINAL_SCORE();
    //         }
    //     }
    // }

    /// <summary>
    /// /////////////////////////////// TESTTTTTTTTT
    /// </summary>
    // private void ProceedToNext()
    // {
    //     if (GameManager.currentGamePageIndex == 0)
    //     {
    //         _uiController.Set_RUNNING_STATE_NEW_CHAPTER();

    //     }
    //     else
    //     {
    //         print("------------ FINITO!!!!!!!!!!!");

    //         /// Game is finished!!!
    //         GameManager.currentGameChapterIndex = 0;
    //         GameManager.currentGamePageIndex = -1;

    //         /// Set my Player
    //         // _myPlayer.NetworkedState = PlayerController.STATE.FINISHED;
    //         _myPlayer.Set_RUNNING_STATE_NONE();

    //         /// UI
    //         _uiController.Set_RUNNING_STATE_FINAL_SCORE();

    //     }
    // }



    public override void OnPlayerRunningStateChanged(int playerId, PlayerController.RUNNING_STATE runningState)
    {
        if (_myPlayer.NetworkedState != PlayerController.STATE.RUNNING) return;

        print("************  RICEVUTO CHANGE RUNNING STATE DA PLAYER: " + playerId + " --> " + runningState.ToString());

        switch (runningState)
        {
            case PlayerController.RUNNING_STATE.THINKING:

                // // print("---------------------------");
                // if (playerId == _myPlayer.NetworkedPlayerId)
                // {
                //     // print("ADESSO SONO IN THINKING...");

                //     GameManager.currentGamePageIndex++;
                //     ProceedToNext();
                // }
                break;

            case PlayerController.RUNNING_STATE.FINISHED:

                if (playerId == _myPlayer.NetworkedPlayerId)
                {
                    if (GameManager.gameSessionData.numberOfPlayersRunning == 1 ||
                         _otherPlayer.NetworkedRunningState == PlayerController.RUNNING_STATE.FINISHED)
                    {
                        /// I have finished too, now we can move on!
                        /// 
                        /// 
                        /// 
                        /// 
                        /// 
                        // ContinueInGame();
                        _uiControllers[0].Set_RUNNING_STATE_CLOSE_PAGE(() =>
                        {
                            print("OOOOOOOOOOOOOOOOOOOOOO");
                        });
                    }
                    else
                    {
                        /// I have finished, but I have to wait the other Player...
                    }
                }
                else if (playerId == _otherPlayer.NetworkedPlayerId)
                {
                    if (_myPlayer.NetworkedRunningState == PlayerController.RUNNING_STATE.FINISHED)
                    {
                        /// Other player have finished too, now we can move on!
                        ///
                        /// 
                        /// 
                        /// 
                        /// 
                        // ContinueInGame();
                        _uiControllers[0].Set_RUNNING_STATE_CLOSE_PAGE(() =>
                        {
                            print("OOOOOOOOOOOOOOOOOOOOOO");
                        });
                    }
                    else
                    {
                        /// Other Player finished, but not me. He's waiting...
                    }
                }
                break;
        }
    }

    /// <summary>
    /// We use this Method, instead detecting the RUNNING_STATE_THINKING over the network,
    /// because it would seem to work only with a delay...
    /// </summary>
    private async void ContinueInGame()
    {
        await Task.Delay(250);
        _myPlayer.Set_RUNNING_STATE_THINKING();
        GameManager.currentGamePageIndex++;
        ProceedToNext();
    }

    //#region APP LOGICS

    /// <summary>
    /// CHECK FOR PLAYERS "STATE" CHANGED
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="state"></param>
    public override void OnPlayerStateChanged(int playerId, PlayerController.STATE state)
    {
        Debug.Log(this.name + " RECEIVED STATE CHANGED FROM PLAYER " + playerId + " ---> " + state);

        switch (state)
        {
            case PlayerController.STATE.IDLE:

                /// Some Player clicked on the Button
                /// of "FINISHED FOR ALL" Panel

                if (playerId == _myPlayer.NetworkedPlayerId)
                {
                    print("Rimetto in READY_TO_START il mio!!!!!!");
                    _uiControllers[0].Set_STATE_READY_TO_START();
                }
                else if (playerId == _otherPlayer.NetworkedPlayerId)
                {
                    print("L'altro PLAYER è in READY_TO_START, quindi metto anche il mio, che attualmente è in: " + _myPlayer.NetworkedState.ToString());

                    /// If we are in Game...
                    if (_myPlayer.NetworkedState != PlayerController.STATE.IDLE)
                        _myPlayer.Set_STATE_IDLE();

                    /// I suppose we are waiting for other Player to finish...
                    else
                        _uiControllers[0].Set_STATE_READY_TO_START();
                }
                break;

            case PlayerController.STATE.RUNNING:

                /// I'm starting the Game for my own Player
                if (playerId == _myPlayer.NetworkedPlayerId)
                {
                    _uiControllers[0].Set_STATE_IN_GAME();

                    // _myPlayer.Set_RUNNING_STATE_THINKING();
                    ContinueInGame();
                }
                /// Other Player started the Game
                else if (playerId == _otherPlayer.NetworkedPlayerId)
                {
                    if (_otherPlayer.NetworkedSessionRequestedPlayers == 1)
                    {
                        print("L'ALTRO PLAYER VUOLE GIOCARE DA SOLO");
                        _uiControllers[0].Set_STATE_WAITING_FOR_PLAYERS();
                    }
                    else if (_otherPlayer.NetworkedSessionRequestedPlayers == 2)
                    {
                        print("SICCOME L'ALTRO PLAYER E' RUNNING, METTO IN RUNNING ANCHE IL MIO, CON id " + _myPlayer.NetworkedPlayerId);
                        _myPlayer.Set_STATE_RUNNING(runningPlayersNumber: 2);
                    }
                }
                break;
        }
    }



    //#region
    /// GENERAL APP LOGICS

    /// <summary>
    /// CHECK FOR PLAYERS (ONLY REAL PLAYERS) COUNT CHANGE
    /// This happens only when someone shutdown or restart
    /// </summary>
    public override void OnPlayersCountChanged()
    {
        /// don't forget to call the base in this function
        base.OnPlayersCountChanged();

        /// retrieve my and other player
        int i = 0;
        print("-------------- OnPlayersCountChanged -----------------");
        foreach (var p in _players)
        {
            print("Player n. " + i + " --- ID: " + p.NetworkedPlayerId);
            if (p.HasStateAuthority) _myPlayer = p;
            else _otherPlayer = p;

            i++;
        }

        /// Too less Players
        if (_players.Count < GameManager.userData.requestedPlayers)
        {
            print("<<<<<<<<<<< NON C'E' il NUMERO DI UTENTI RICHIESTO: " + _players.Count + "/" + GameManager.userData.requestedPlayers);

            /// Set UI
            if (_uiControllers[0].state != UiControllerBase.STATE.WAITING_FOR_PLAYERS)
                _uiControllers[0].Set_STATE_WAITING_FOR_PLAYERS();
        }

        /// Right number of Players!
        else if (_players.Count == GameManager.userData.requestedPlayers)
        {
            if (_otherPlayer != null)
            {
                if (_otherPlayer.NetworkedSessionRequestedPlayers == 1) return;
            }

            print(">>>>>>>>>>>>>>> C'E' IL NUMERO DI UTENTI RICHIESTO, VERIFICHIAMO I LORO ID...");

            /// Reset the other Player to null
            if (_players.Count == 1) _otherPlayer = null;

            if (_myPlayer != null && _otherPlayer != null && _otherPlayer.NetworkedPlayerId == _myPlayer.NetworkedPlayerId)
            {
                GameManager.instance.ShowModal("ERRORE", "Ci sono due players con lo stesso ID", showConfigureButton: true, showRestartButton: false);
            }
            else
            {
                print("IL MIO PLAYER ID E' : " + _myPlayer.NetworkedPlayerId);

                /// If we changed something that require others to restart,
                /// send message to restart!
                if (GameManager.instance.sendMessageToRestart)
                {
                    GameManager.instance.sendMessageToRestart = false;
                    _myPlayer.SendMessageToRestart();
                }

                /// Set UI IDLE
                _uiControllers[0].Set_STATE_READY_TO_START();
            }
        }
    }



}
