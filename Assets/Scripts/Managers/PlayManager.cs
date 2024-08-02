using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayManager : NetworkManagerBase
{
    public PlayerController otherPlayer { get; private set; } = null;
    private int _playersCount;



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

    //#endregion





    //#region APP LOGICS

    /// <summary>
    /// CHECK FOR PLAYERS "STATE" CHANGED
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="state"></param>
    public override async void OnPlayerStateChanged(int playerId, PlayerController.STATE state)
    {
        Debug.Log("<color=yellow>PlayManager - RunningStateChanged: </color>" + " - Player " + playerId + " - " + state.ToString());

        switch (state)
        {
            /// After the INTRO, and FINAL_SCORE
            case PlayerController.STATE.READY:

                if (otherPlayer != null && myPlayer.NetworkedState == otherPlayer.NetworkedState)
                {
                    /// We both are in READY state, let's show the READY UI
                    _uiControllers[0].Set_STATE_READY_TO_START();

                }
                else if (otherPlayer == null && GameManager.userData.requestedPlayers == 1)
                {
                    /// I'm in "SOLO" mode, let's show the READY UI
                    _uiControllers[0].Set_STATE_READY_TO_START();
                }
                break;

            case PlayerController.STATE.RUNNING:

                print("------------------------------------");
                print("---------- INIZIA IL GIOCO -----------");
                print("------------------------------------");

                GameManager.currentGameChapterIndex = 0;
                GameManager.currentGamePageIndex = -1;

                /// I'm starting the Game for my own Player
                if (playerId == myPlayer.NetworkedId)
                {
                    _uiControllers[0].Set_STATE_IN_GAME();

                    /// Here we must wait for some time, I think because we have just received
                    /// the State change... If we not wait on VIEWER version will be called 3 times!!!
                    await Task.Delay(GameManager.instance.FusionDelayTime);
                    myPlayer.Set_RUNNING_STATE_THINKING();
                }
                /// Other Player started the Game
                else if (playerId == otherPlayer.NetworkedId)
                {
                    if (otherPlayer.NetworkedSessionRequestedPlayers == 1)
                    {
                        /// Other player want to play alone
                        // print("L'ALTRO PLAYER VUOLE GIOCARE DA SOLO");
                        _uiControllers[0].Set_STATE_WAITING_FOR_PLAYERS();
                    }
                    else if (otherPlayer.NetworkedSessionRequestedPlayers == 2)
                    {
                        /// Other player want to play with me
                        // print("SICCOME L'ALTRO PLAYER E' RUNNING, METTO IN RUNNING ANCHE IL MIO, CON id " + myPlayer.NetworkedId);
                        myPlayer.Set_STATE_RUNNING(runningPlayersNumber: 2);
                    }
                }
                break;
        }
    }

    //#endregion







    //#region GAME LOGICS
    public override async void OnPlayerRunningStateChanged(int playerId, PlayerController.RUNNING_STATE runningState)
    {
        if (myPlayer.NetworkedState != PlayerController.STATE.RUNNING) return;

        Debug.Log("<color=orange>PlayManager - RunningStateChanged: </color>" + " - Player " + playerId + " - " + runningState.ToString());

        switch (runningState)
        {
            case PlayerController.RUNNING_STATE.NONE:

                /// The game is finished
                if (playerId == myPlayer.NetworkedId)
                {
                    // GameManager.currentGameChapterIndex = 0;
                    // GameManager.currentGamePageIndex = -1;
                    _uiControllers[0].Set_STATE_FINAL_SCORE();
                }
                break;

            case PlayerController.RUNNING_STATE.THINKING:

                if (playerId == myPlayer.NetworkedId)
                {
                    GameManager.currentGamePageIndex++;

                    GameManager.instance.GetNewGameState(async (gameState) =>
                    {
                        switch (gameState)
                        {
                            case GameManager.GAME_STATE.CHAPTER:
                                _uiControllers[0].Set_RUNNING_STATE_OPEN_CHAPTER(() => _uiControllers[0].Set_RUNNING_STATE_OPEN_PAGE());
                                break;

                            case GameManager.GAME_STATE.PAGE:
                                _uiControllers[0].Set_RUNNING_STATE_OPEN_PAGE();
                                break;

                            case GameManager.GAME_STATE.END:
                                print("IL GIOCO E' FINiTO!!!");

                                await Task.Delay(GameManager.instance.FusionDelayTime);
                                myPlayer.Set_RUNNING_STATE_NONE();
                                break;
                        }
                    });
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
                        print("YEEEEEEEEE, HO FINITO ANCH'IO....");
                        await Task.Delay(GameManager.instance.FusionDelayTime);
                        myPlayer.Set_RUNNING_STATE_THINKING();
                    }
                    else
                    {
                        /// I have finished, but I have to wait the other Player...
                        print("CHE PALLE... MI TOCCA ASPETTARE...");
                        _uiControllers[0].Set_RUNNING_STATE_WAIT_OTHER_PLAYER();
                    }
                }
                else if (playerId == otherPlayer.NetworkedId)
                {
                    if (myPlayer.NetworkedRunningState == PlayerController.RUNNING_STATE.FINISHED)
                    {
                        /// Other player have finished too, now we can move on!
                        print("....FINALMENTE HA FINITO ANCHE LUI....");
                        await Task.Delay(GameManager.instance.FusionDelayTime);
                        myPlayer.Set_RUNNING_STATE_THINKING();
                    }
                    else
                    {
                        /// Other Player finished, but not me. He's waiting...
                    }
                }
                break;
        }
    }
    //#endregion



    //#region
    /// GENERAL APP LOGICS

    /// <summary>
    /// CHECK FOR PLAYERS (ONLY REAL PLAYERS, NOT THE VIEWER) COUNT CHANGE
    /// This happens only when someone shutdown or start
    /// </summary>
    public override void OnRealPlayersCountChanged()
    {
        /// Retrieve the list of actual connected Players
        base.OnRealPlayersCountChanged();

        // Debug.Log("<color=orange>PlayerManager - OnPlayersCountChanged: </color>");

        /// We want to skip when the Viewer join or leave
        if (players.Count == _playersCount) return;
        _playersCount = players.Count;


        foreach (var p in players)
        {
            /// Check if there's some weird error on ID assignment
            /// This should NEVER happen!!! (But happened!!!!)
            if (p.NetworkedId < 0 || p.NetworkedId > 1)
            {
                GameManager.instance.ShowModal("ERRORE DI SISTEMA", "C'e' stato un problema con l'assegnazione degli ID. Questo non dovrebbe succedere. Si prega di riavviare tutto e segnalare il problema agli sviluppatori",
                showConfigureButton: false, showRestartButton: true);
                return;
            }

            /// I've entered for the 1st time!
            if (myPlayer == null && p.HasStateAuthority)
            {
                /// Show INTRO UI
                _uiControllers[0].Set_STATE_INTRO(() =>
                {
                    /// Get my Player
                    myPlayer = p;

                    WaitForPlayers = StartCoroutine(WaitForPlayersCoroutine());
                });
            }
        }

        /// After that I've set myPlayer, let's check if
        /// other player leave
        if (myPlayer != null && WaitForPlayers == null)
        {
            WaitForPlayers = StartCoroutine(WaitForPlayersCoroutine());
        }
    }

    private Coroutine WaitForPlayers;

    private IEnumerator WaitForPlayersCoroutine()
    {
        myPlayer.NetworkedState = PlayerController.STATE.NONE;
        otherPlayer = null;

        /// Show WAITING UI
        _uiControllers[0].Set_STATE_WAITING_FOR_PLAYERS();

        /// Let's wait the end of the animation
        yield return new WaitForSeconds(1);

        /// If we changed something that require others to restart,
        /// send message to restart!
        if (GameManager.instance.sendMessageToRestart)
        {
            myPlayer.SendMessageToRestart();

            /// Wait for the message is sent and received 
            while (GameManager.instance.sendMessageToRestart == true)
                yield return null;
        }

        /// Too many players
        if (players.Count > GameManager.userData.requestedPlayers)
            GameManager.instance.ShowNotification("Si sono aggiunti troppi utenti. Il massimo consentito è " + GameManager.userData.requestedPlayers);

        /// Wait for the right number of players
        while (players.Count != GameManager.userData.requestedPlayers)
            yield return null;

        /// Get other Player
        if (GameManager.userData.requestedPlayers > 1)
            otherPlayer = players.Find(elem => elem.HasStateAuthority == false);

        /// Check for error due to the same player ID assignment
        if (otherPlayer != null && otherPlayer.NetworkedId == myPlayer.NetworkedId)
        {
            GameManager.instance.ShowModal("ERRORE", "Ci sono due players con lo stesso ID", showConfigureButton: true, showRestartButton: false);
            yield break;
        }

        /// Set myPlayer to READY
        myPlayer.NetworkedState = PlayerController.STATE.READY;

        WaitForPlayers = null;
    }



}
