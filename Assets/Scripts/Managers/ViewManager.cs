using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : NetworkManagerBase
{
    private List<PlayerController> _runningPlayers;


    protected override void Started()
    {
        foreach (var ui in _uiControllers)
        {
            ui.Set_STATE_INTRO(() => ui.Set_STATE_READY_TO_START());
        }
        // _uiControllers[0].Set_STATE_INTRO(() => { });
        // StartCoroutine(test());
    }

    // IEnumerator test()
    // {
    //     _uiControllers[1].Set_STATE_INTRO(() => { });
    //     yield return new WaitForSeconds(3);
    //      _uiControllers[0].Set_STATE_INTRO(() => { });
    // }


    //#region APP LOGICS

    /// <summary>
    /// CHECK FOR PLAYERS "STATE" CHANGED
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="state"></param>
    public override void OnPlayerStateChanged(int playerId, PlayerController.STATE playerState)
    {
        Debug.Log(this.name + " RECEIVED STATE CHANGED FROM PLAYER " + playerId + " ---> " + playerState);

        /// Refresh the list of real Players (this is necessary if the Viewer start up after the Player)
        OnRealPlayersCountChanged();

        switch (playerState)
        {
            case PlayerController.STATE.READY:

                if (_uiControllers[playerId].state != UiController.STATE.READY_TO_START)
                    _uiControllers[playerId].Set_STATE_READY_TO_START();
                break;

            case PlayerController.STATE.RUNNING:

                _runningPlayers = new List<PlayerController>(_players.FindAll(x => x.NetworkedState == PlayerController.STATE.RUNNING));

                // print("I players sono: " + _players.Count);

                GameManager.gameSessionData = new Data.GameSessionData
                {
                    // numberOfPlayersRunning = _players[playerId].NetworkedSessionRequestedPlayers
                    numberOfPlayersRunning = _runningPlayers.Count
                };

                _uiControllers[playerId].Set_STATE_IN_GAME();
                break;

        }
    }

    //#endregion





    //#region GAME LOGICS
    private int _playersInThinking = 0;
    public override void OnPlayerRunningStateChanged(int playerId, PlayerController.RUNNING_STATE runningState)
    {
        // if (_players[playerId].NetworkedState != PlayerController.STATE.RUNNING) return;

        print("************  RICEVUTO CHANGE RUNNING STATE DA PLAYER: " + playerId + " --> " + runningState.ToString());

        switch (runningState)
        {
            case PlayerController.RUNNING_STATE.NONE:

                /// The game is finished
                GameManager.currentGameChapterIndex = 0;
                GameManager.currentGamePageIndex = -1;
                _uiControllers[playerId].Set_STATE_FINAL_SCORE();
                break;

            case PlayerController.RUNNING_STATE.THINKING:

                _playersInThinking++;

                if (_playersInThinking == 1)
                {
                    print("AVANZO DI UNA PAGINA");
                    GameManager.currentGamePageIndex++;

                    if (_runningPlayers.Count == 1)
                    {
                        print("RESETTO X' c'è uno solo running player");
                        _playersInThinking = 0;
                    }
                }
                else if (_playersInThinking == 2)
                {
                    print("RESETTO X' anche l'altro è in thinking");
                    _playersInThinking = 0;
                }

                GameManager.instance.GetNewGameState((gameState) =>
                {
                    switch (gameState)
                    {
                        case GameManager.GAME_STATE.CHAPTER:
                            _uiControllers[playerId].Set_RUNNING_STATE_OPEN_CHAPTER(() => _uiControllers[playerId].Set_RUNNING_STATE_OPEN_PAGE());
                            break;

                        case GameManager.GAME_STATE.PAGE:
                            _uiControllers[playerId].Set_RUNNING_STATE_OPEN_PAGE();
                            break;

                            // case GameManager.GAME_STATE.FINISHED:
                            //     break;
                    }
                });

                // // /// Since both players will send this state, we must avoid a double call
                // // if (GameManager.gameSessionData.numberOfPlayersRunning == 2)
                // // {
                // if (_runningPlayers.Count == 1)
                // {
                //     print("AVANZO DI UNA PAGINA PERCHE' SONO DA SOLO");
                //     GameManager.currentGamePageIndex++;
                // }
                // else
                // {
                //     if (_players[0].NetworkedRunningState != _players[1].NetworkedRunningState)
                //     {
                //         print(_players[0].NetworkedRunningState + " ---- " + _players[1].NetworkedRunningState);
                //         print("AVANZO DI UNA PAGINA PERCHE' SONO IL PRIMO");
                //         GameManager.currentGamePageIndex++;
                //     }
                //     else
                //     {
                //         print("NON AVANZO!!!!");
                //     }
                // }


                // }


                // ProceedToNext();





                // ///
                // /// ////////////////////////////////////
                // if (GameManager.currentGamePageIndex <= GameManager.currentGameChapter.pages.Count - 1)
                // {
                //     if (GameManager.currentGamePageIndex == 0)
                //         _uiControllers[playerId].Set_RUNNING_STATE_OPEN_CHAPTER(() => _uiControllers[playerId].Set_RUNNING_STATE_OPEN_PAGE());
                //     else
                //         _uiControllers[playerId].Set_RUNNING_STATE_OPEN_PAGE();
                // }
                // else
                // {
                //     if (GameManager.currentGameChapterIndex < GameManager.currentGameVersion.chapters.Count - 1)
                //     {
                //         /// Iterate
                //         GameManager.currentGameChapterIndex++;
                //         GameManager.currentGamePageIndex = 0;

                //         ProceedToNext();
                //     }
                //     // else
                //     // {
                //     //     /// Player
                //     //     if (myPlayer != null) myPlayer.Set_RUNNING_STATE_NONE();

                //     //     /// Viewer
                //     //     else foreach (var p in _players) p.Set_RUNNING_STATE_NONE();
                //     // }
                // }

                /// <summary>
                /// ///////////////////////////
                /// </summary>

                break;


            case PlayerController.RUNNING_STATE.CLICKED:

                /// Some player have clicked on the answer
                _uiControllers[playerId].Set_RUNNING_STATE_ANSWER_CLICKED(() => { });

                break;


            case PlayerController.RUNNING_STATE.FINISHED:

                var playerThatHasNotFinished = _runningPlayers.Find(x => x.NetworkedRunningState != PlayerController.RUNNING_STATE.FINISHED);

                if (playerThatHasNotFinished == null)
                {
                    /// All running players have finished
                    for (int i = 0; i < _runningPlayers.Count; i++)
                        _uiControllers[_runningPlayers[i].NetworkedId].Set_RUNNING_STATE_CLOSE_PAGE(() => { });
                }
                else
                {
                    /// One player have finished, but it have to wait the other Player...
                    _uiControllers[playerId].Set_RUNNING_STATE_WAIT_OTHER_PLAYER();
                }





                // if (playerId == myPlayer.NetworkedId)
                // {
                //     if (GameManager.gameSessionData.numberOfPlayersRunning == 1 ||
                //          otherPlayer.NetworkedRunningState == PlayerController.RUNNING_STATE.FINISHED)
                //     {
                //         /// I have finished too, now we can move on!
                //         _uiControllers[playerId].Set_RUNNING_STATE_CLOSE_PAGE(() => { });
                //     }
                //     else
                //     {
                //         /// I have finished, but I have to wait the other Player...
                //         _uiControllers[playerId].Set_RUNNING_STATE_WAIT_OTHER_PLAYER();
                //     }
                // }
                // else if (playerId == otherPlayer.NetworkedId)
                // {
                //     if (myPlayer.NetworkedRunningState == PlayerController.RUNNING_STATE.FINISHED)
                //     {
                //         /// Other player have finished too, now we can move on!
                //         _uiControllers[playerId].Set_RUNNING_STATE_CLOSE_PAGE(() => { });

                //     }
                //     else
                //     {
                //         /// Other Player finished, but not me. He's waiting...
                //     }
                // }
                break;
        }
    }
    //#endregion




    //#region
    /// GENERAL APP LOGICS

    /// <summary>
    /// CHECK FOR PLAYERS (ONLY REAL PLAYERS) COUNT CHANGE
    /// </summary>
    public override void OnRealPlayersCountChanged()
    {
        /// don't forget to call the base in this function
        base.OnRealPlayersCountChanged();

        print("ADESSO CE NE SONO: " + _players.Count);
    }




}
