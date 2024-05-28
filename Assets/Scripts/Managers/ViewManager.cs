using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : NetworkManagerBase
{
    // [SerializeField] UiViewController[] _uiControllers;

    // private enum GAMESTATE { IDLE, RUNNING, FINISHED }
    // private GAMESTATE gameState;
    // private PlayerController.PLAYERSTATE[] playerStateList = new PlayerController.PLAYERSTATE[2];


    // public override void Started()
    // {


    //     IdleGame();
    // }


    public override void OnPlayerStateChanged(int playerId, PlayerController.STATE playerState)
    {
        Debug.Log(this.name + " RECEIVED STATE CHANGED FROM PLAYER " + playerId + " ---> " + playerState);


        switch (playerState)
        {
            case PlayerController.STATE.IDLE:

                _uiControllers[playerId].Set_STATE_READY_TO_START();
                break;

            case PlayerController.STATE.RUNNING:

                // PlayerController runningPlayer = null;
                // foreach(var p in players) if (p.NetworkedPlayerId == playerId) runningPlayer = p;

                PlayerController runningPlayer = _players.Find(x => x.NetworkedPlayerId == playerId);

                GameManager.gameSessionData = new Data.GameSessionData
                {
                    numberOfPlayersRunning = runningPlayer.NetworkedSessionRequestedPlayers
                };

                _uiControllers[playerId].Set_STATE_IN_GAME();
                break;

            // case PlayerController.STATE.FINISHED:

            //     if (GameManager.gameSessionData.numberOfPlayersRunning == 1)
            //     {
            //         _uiControllers[playerId].SetState(GamePanelControllerBase.STATE.FINISHED_FOR_ALL);
            //     }
            //     else
            //     {
            //         if (players[0].NetworkedState == players[1].NetworkedState)
            //         {
            //             foreach (var ctrl in _uiControllers)
            //                 ctrl.SetState(GamePanelControllerBase.STATE.FINISHED_FOR_ALL);
            //         }
            //         else
            //         {
            //             _uiControllers[playerId].SetState(GamePanelControllerBase.STATE.FINISHED_FOR_ME);
            //         }
            //     }
            //     break;
        }
    }


    // public override void OnPlayerScoreChanged(int playerId, float value)
    // {
    //     Debug.Log(this.name + " RECEIVED SCORE CHANGED FROM PLAYER " + playerId + " ---> " + value);
    // }



    /// <summary>
    /// CHECK FOR PLAYERS (ONLY REAL PLAYERS) COUNT CHANGE
    /// </summary>
    public override void OnPlayersCountChanged()
    {
        /// don't forget to call the base in this function
        base.OnPlayersCountChanged();


    }


    // /// <summary>
    // /// GAME IDLE
    // /// </summary>
    // private void IdleGame()
    // {
    //     // gameState = GAMESTATE.IDLE;
    //     Debug.Log("GAME IS IDLE...");
    // }

    // /// <summary>
    // /// GAME STARTED
    // /// </summary>
    // private void StartGame()
    // {
    //     // gameState = GAMESTATE.RUNNING;
    //     Debug.Log("GAME STARTED!!!!!");
    // }

    // /// <summary>
    // /// GAME FINISHED
    // /// </summary>
    // private void EndGame()
    // {
    //     // gameState = GAMESTATE.FINISHED;
    //     Debug.Log("GAME FINISHED!!!!!");
    // }


}
