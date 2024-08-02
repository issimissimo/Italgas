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
    public override void OnPlayerStateChanged(int playerId, PlayerController.STATE state)
    {
        Debug.Log("<color=yellow>Player with ID: " + playerId + " changed its STATE to: " + state.ToString() + "</color>");

        /// Refresh the list of real Players (this is necessary if the Viewer start up after the Player)
        /// AND set (maybe) the UI to READY 
        OnRealPlayersCountChanged();

        switch (state)
        {
            case PlayerController.STATE.RUNNING:

                print("------------------------------------");
                print("---------- INIZIA IL GIOCO -----------");
                print("------------------------------------");

                GameManager.currentGameChapterIndex = 0;
                GameManager.currentGamePageIndex = -1;
                
                _runningPlayers = new List<PlayerController>(players.FindAll(x => x.NetworkedState == PlayerController.STATE.RUNNING));

                GameManager.gameSessionData = new Data.GameSessionData
                {
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

        Debug.Log("<color=orange>Player with ID: " + playerId + " changed its RUNNING STATE to: " + runningState.ToString() + "</color>");

        switch (runningState)
        {
            case PlayerController.RUNNING_STATE.NONE:

                /// The game is finished
                // GameManager.currentGameChapterIndex = 0;
                // GameManager.currentGamePageIndex = -1;
                _uiControllers[playerId].Set_STATE_FINAL_SCORE();
                break;

            case PlayerController.RUNNING_STATE.THINKING:

                _playersInThinking++;

                print("SONO IN THINKING...");

                if (_playersInThinking == 1)
                {
                    GameManager.currentGamePageIndex++;
                    print("... E AVANZO DI UNA PAGINA Adesso: currentGameChapterIndex = " + GameManager.currentGameChapterIndex + " --- currentGamePageIndex = " + GameManager.currentGamePageIndex);

                    if (_runningPlayers.Count == 1)
                    {
                        // print("RESETTO X' c'è uno solo running player");
                        _playersInThinking = 0;
                    }
                }
                else if (_playersInThinking == 2)
                {
                    // print("RESETTO X' anche l'altro è in thinking");
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



                break;


            case PlayerController.RUNNING_STATE.CLICKED:

                /// Some player have clicked on the answer
                _uiControllers[playerId].Set_RUNNING_STATE_ANSWER_CLICKED(() => { });

                break;


            case PlayerController.RUNNING_STATE.FINISHED:

                var playerThatHasNotFinished = _runningPlayers.Find(x => x.NetworkedRunningState != PlayerController.RUNNING_STATE.FINISHED);

                if (playerThatHasNotFinished != null)
                {
                    /// One player have finished, but it have to wait the other Player...
                    print("UN PLAYER HA FINITO DI RISPONDERE MA L'ALTRO NO...");
                    _uiControllers[playerId].Set_RUNNING_STATE_WAIT_OTHER_PLAYER();
                }
                else
                {
                    print("TUTTI I PLAYERS HANNO FINITO DI RISPONDERE");
                }
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

        print("ADESSO CE NE SONO: " + players.Count);


        if (players.Count == 0)
        {
            for (int i = 0; i < _uiControllers.Length; i++)
            {
                if (_uiControllers[i].state != UiController.STATE.READY_TO_START)
                    _uiControllers[i].Set_STATE_READY_TO_START();
            }
        }

        else if (players.Count == 1)
        {
            for (int i = 0; i < _uiControllers.Length; i++)
            {
                if (i == players[0].NetworkedId)
                {
                    if (players[0].NetworkedState == PlayerController.STATE.READY && _uiControllers[i].state != UiController.STATE.READY_TO_START)
                        _uiControllers[i].Set_STATE_READY_TO_START();
                }
                else
                {
                    if (_uiControllers[i].state != UiController.STATE.READY_TO_START)
                        _uiControllers[i].Set_STATE_READY_TO_START();
                }
            }
        }

        else if (players.Count == 2)
        {
            for (int i = 0; i < players.Count; i++)
            {
                int playerId = players[i].NetworkedId;
                if (players[i].NetworkedState == PlayerController.STATE.READY && _uiControllers[playerId].state != UiController.STATE.READY_TO_START)
                    _uiControllers[playerId].Set_STATE_READY_TO_START();
            }
        }
    }




}
