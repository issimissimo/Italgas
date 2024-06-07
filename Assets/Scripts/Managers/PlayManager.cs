using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayManager : NetworkManagerBase
{
    public PlayerController otherPlayer { get; private set; } = null;



    //#region INTERACTIONS FROM UI

    /// <summary>
    /// CLICKED ON THE ANSWER IN THE UI
    /// </summary>
    /// <param name="buttonNumber"></param> <summary>
    public void OnAnswerButtonPressed(int buttonNumber, bool isTrue, float time)
    {
        myPlayer.Set_RUNNING_STATE_CLICKED(buttonNumber, isTrue, time);
    }



    /// <summary>
    /// BUTTON
    /// </summary>
    public void Set_RUNNING()
    {
        myPlayer.Set_STATE_RUNNING();
    }

    /// <summary>
    /// BUTTON
    /// </summary>
    public void Set_IDLE()
    {
        myPlayer.Set_STATE_IDLE();
    }










    //#region GAME LOGICS
    public override void OnPlayerRunningStateChanged(int playerId, PlayerController.RUNNING_STATE runningState)
    {
        if (myPlayer.NetworkedState != PlayerController.STATE.RUNNING) return;

        print("************  RICEVUTO CHANGE RUNNING STATE DA PLAYER: " + playerId + " --> " + runningState.ToString());

        switch (runningState)
        {
            case PlayerController.RUNNING_STATE.NONE:

                /// The game is finished
                GameManager.currentGameChapterIndex = 0;
                GameManager.currentGamePageIndex = -1;
                _uiControllers[0].Set_STATE_FINAL_SCORE();
                break;

            case PlayerController.RUNNING_STATE.THINKING:

                if (playerId == myPlayer.NetworkedId)
                {
                    GameManager.currentGamePageIndex++;
                    ProceedToNext();
                }
                break;


            case PlayerController.RUNNING_STATE.CLICKED:
                if (playerId == myPlayer.NetworkedId)
                {
                    /// I have clicked on the answer
                    _uiControllers[0].Set_RUNNING_STATE_ANSWER_CLICKED(() => myPlayer.Set_RUNNING_STATE_FINISHED());
                }
                break;


            case PlayerController.RUNNING_STATE.FINISHED:

                if (playerId == myPlayer.NetworkedId)
                {
                    if (GameManager.gameSessionData.numberOfPlayersRunning == 1 ||
                         otherPlayer.NetworkedRunningState == PlayerController.RUNNING_STATE.FINISHED)
                    {
                        /// I have finished too, now we can move on!
                        _uiControllers[0].Set_RUNNING_STATE_CLOSE_PAGE(() => myPlayer.Set_RUNNING_STATE_THINKING());
                    }
                    else
                    {
                        /// I have finished, but I have to wait the other Player...
                        _uiControllers[0].Set_RUNNING_STATE_WAIT_OTHER_PLAYER();
                    }
                }
                else if (playerId == otherPlayer.NetworkedId)
                {
                    if (myPlayer.NetworkedRunningState == PlayerController.RUNNING_STATE.FINISHED)
                    {
                        /// Other player have finished too, now we can move on!
                        _uiControllers[0].Set_RUNNING_STATE_CLOSE_PAGE(() => myPlayer.Set_RUNNING_STATE_THINKING());

                    }
                    else
                    {
                        /// Other Player finished, but not me. He's waiting...
                    }
                }
                break;
        }
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
            case PlayerController.STATE.READY:

                /// After the INTRO, and FINAL_SCORE
                
                if (otherPlayer != null)
                {
                    if (myPlayer.NetworkedState == otherPlayer.NetworkedState)
                    {
                        /// We both are in READY state, let's show the READY UI
                        _uiControllers[0].Set_STATE_READY_TO_START();
                    }
                }
                else if (otherPlayer == null)
                {
                    /// I'm in "SOLO" mode, let's show the READY UI
                    _uiControllers[0].Set_STATE_READY_TO_START();
                }


                // if (playerId == myPlayer.NetworkedId)
                // {
                //     print("Metto in READY_TO_START il mio!!!!!!");
                //     _uiControllers[0].Set_STATE_READY_TO_START();
                // }
                // else if (otherPlayer != null && playerId == otherPlayer.NetworkedId)
                // {
                //     print("L'altro PLAYER è in READY_TO_START, quindi metto anche il mio, se non lo è già");

                //     myPlayer.Set_STATE_IDLE();
                // }
                break;

            case PlayerController.STATE.RUNNING:

                /// I'm starting the Game for my own Player
                if (playerId == myPlayer.NetworkedId)
                {
                    _uiControllers[0].Set_STATE_IN_GAME();
                    myPlayer.Set_RUNNING_STATE_THINKING();
                    // ContinueInGame();
                }
                /// Other Player started the Game
                else if (playerId == otherPlayer.NetworkedId)
                {
                    if (otherPlayer.NetworkedSessionRequestedPlayers == 1)
                    {
                        /// Other player want to play alone
                        print("L'ALTRO PLAYER VUOLE GIOCARE DA SOLO");
                        _uiControllers[0].Set_STATE_WAITING_FOR_PLAYERS();
                    }
                    else if (otherPlayer.NetworkedSessionRequestedPlayers == 2)
                    {
                        /// Other player want to play with me
                        print("SICCOME L'ALTRO PLAYER E' RUNNING, METTO IN RUNNING ANCHE IL MIO, CON id " + myPlayer.NetworkedId);
                        myPlayer.Set_STATE_RUNNING(runningPlayersNumber: 2);
                    }
                }
                break;
        }
    }



    //#region
    /// GENERAL APP LOGICS

    /// <summary>
    /// CHECK FOR PLAYERS (ONLY REAL PLAYERS, NOT THE VIEWER) COUNT CHANGE
    /// This happens only when someone shutdown or start
    /// </summary>
    public override void OnPlayersCountChanged()
    {
        /// Retrieve the list of actual connected Players
        base.OnPlayersCountChanged();
        print("-------------- OnPlayersCountChanged -----------------");

        foreach (var p in _players)
        {
            /// Check if there's some weird error on ID assignment
            /// This should NEVER happen!!!
            if (p.NetworkedId < 0 || p.NetworkedId > 1)
            {
                GameManager.instance.ShowModal("ERRORE", "C'e' stato un problema con l'assegnazione degli ID ID. Questo non dovrebbe succedere. Si prega di segnalare il problema agli sviluppatori",
                showConfigureButton: false, showRestartButton: true);
                return;
            }

            if (p.HasStateAuthority)
            {
                if (myPlayer == null)
                {
                    myPlayer = p;

                    /// Show INTRO
                    _uiControllers[0].Set_STATE_INTRO(() => StartCoroutine(WaitTheRightPlayersNumber()));
                }
            }
        }




        // /// Too less Players
        // if (_players.Count < GameManager.userData.requestedPlayers)
        // {
        //     print("<<<<<<<<<<< NON C'E' il NUMERO DI UTENTI RICHIESTO: " + _players.Count + "/" + GameManager.userData.requestedPlayers);

        //     /// Set UI
        //     if (_uiControllers[0].state != UiController.STATE.WAITING_FOR_PLAYERS)
        //         _uiControllers[0].Set_STATE_WAITING_FOR_PLAYERS();
        // }

        // /// Right number of Players!
        // else if (_players.Count == GameManager.userData.requestedPlayers)
        // {
        //     if (otherPlayer != null)
        //     {
        //         if (otherPlayer.NetworkedSessionRequestedPlayers == 1) return;
        //     }

        //     print(">>>>>>>>>>>>>>> C'E' IL NUMERO DI UTENTI RICHIESTO, VERIFICHIAMO I LORO ID...");

        //     /// Reset the other Player to null
        //     if (_players.Count == 1) otherPlayer = null;

        //     if (myPlayer != null && otherPlayer != null && otherPlayer.NetworkedId == myPlayer.NetworkedId)
        //     {
        //         GameManager.instance.ShowModal("ERRORE", "Ci sono due players con lo stesso ID", showConfigureButton: true, showRestartButton: false);
        //     }
        //     else
        //     {
        //         print("IL MIO PLAYER ID E' : " + myPlayer.NetworkedId);

        //         /// If we changed something that require others to restart,
        //         /// send message to restart!
        //         if (GameManager.instance.sendMessageToRestart)
        //         {
        //             GameManager.instance.sendMessageToRestart = false;
        //             myPlayer.SendMessageToRestart();
        //         }

        //         /// Set UI IDLE
        //         _uiControllers[0].Set_STATE_READY_TO_START();
        //     }
        // }
    }

    private IEnumerator WaitTheRightPlayersNumber()
    {

        _uiControllers[0].Set_STATE_WAITING_FOR_PLAYERS();

        /// Wait for the right number of players
        while (_players.Count != GameManager.userData.requestedPlayers)
            yield return null;

        /// Get other Player
        otherPlayer = _players.Find(elem => elem.HasStateAuthority == false);

        /// Check for error due to the same player ID assignment
        if (otherPlayer != null && otherPlayer.NetworkedId == myPlayer.NetworkedId)
        {
            GameManager.instance.ShowModal("ERRORE", "Ci sono due players con lo stesso ID", showConfigureButton: true, showRestartButton: false);
            yield break;
        }

        /// Set Player to READY
        print("IL MIO PLAYER ID E' : " + myPlayer.NetworkedId);
        if (otherPlayer == null) print("L'ALTRO PLAYER NON C?E'");
        myPlayer.NetworkedState = PlayerController.STATE.READY;

    }

    // private void CheckCurrentPlayers()
    // {
    //     return;

    //     /// Too less Players
    //     if (_players.Count < GameManager.userData.requestedPlayers)
    //     {
    //         print("<<<<<<<<<<< NON C'E' il NUMERO DI UTENTI RICHIESTO: " + _players.Count + "/" + GameManager.userData.requestedPlayers);

    //         /// Set UI
    //         if (_uiControllers[0].state != UiController.STATE.WAITING_FOR_PLAYERS)
    //             _uiControllers[0].Set_STATE_WAITING_FOR_PLAYERS();
    //     }

    //     /// Right number of Players!
    //     else if (_players.Count == GameManager.userData.requestedPlayers)
    //     {
    //         if (otherPlayer != null)
    //         {
    //             if (otherPlayer.NetworkedSessionRequestedPlayers == 1) return;
    //         }

    //         print(">>>>>>>>>>>>>>> C'E' IL NUMERO DI UTENTI RICHIESTO, VERIFICHIAMO I LORO ID...");

    //         /// Reset the other Player to null
    //         if (_players.Count == 1) otherPlayer = null;

    //         if (myPlayer != null && otherPlayer != null && otherPlayer.NetworkedId == myPlayer.NetworkedId)
    //         {
    //             GameManager.instance.ShowModal("ERRORE", "Ci sono due players con lo stesso ID", showConfigureButton: true, showRestartButton: false);
    //         }
    //         else
    //         {
    //             print("IL MIO PLAYER ID E' : " + myPlayer.NetworkedId);

    //             /// If we changed something that require others to restart,
    //             /// send message to restart!
    //             if (GameManager.instance.sendMessageToRestart)
    //             {
    //                 GameManager.instance.sendMessageToRestart = false;
    //                 myPlayer.SendMessageToRestart();
    //             }

    //             /// Set UI IDLE
    //             _uiControllers[0].Set_STATE_READY_TO_START();
    //         }
    //     }
    // }



}
