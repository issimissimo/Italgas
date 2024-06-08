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
            // ui.Set_STATE_INTRO(()=> ui.Set_STATE_READY_TO_START());
            ui.Set_STATE_INTRO(()=> {});
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


        switch (playerState)
        {
            case PlayerController.STATE.READY:

                _uiControllers[playerId].Set_STATE_READY_TO_START();
                break;

            case PlayerController.STATE.RUNNING:

                // // PlayerController runningPlayer = null;
                // // foreach(var p in players) if (p.NetworkedPlayerId == playerId) runningPlayer = p;

                _runningPlayers = new List<PlayerController>(_players.FindAll(x => x.NetworkedState == PlayerController.STATE.RUNNING));

                // PlayerController runningPlayer = _players.Find(x => x.NetworkedId == playerId);

                GameManager.gameSessionData = new Data.GameSessionData
                {
                    numberOfPlayersRunning = _players[playerId].NetworkedSessionRequestedPlayers
                };

                _uiControllers[playerId].Set_STATE_IN_GAME();
                break;

        }
    }

    //#endregion





    //#region GAME LOGICS
    public override void OnPlayerRunningStateChanged(int playerId, PlayerController.RUNNING_STATE runningState)
    {
        if (_players[playerId].NetworkedState != PlayerController.STATE.RUNNING) return;

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

                if (playerId == myPlayer.NetworkedId)
                {
                    GameManager.currentGamePageIndex++;
                    ProceedToNext();
                }
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


    }




}
