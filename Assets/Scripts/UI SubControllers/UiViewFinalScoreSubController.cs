using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiViewFinalScoreSubController : GamePanelSubControllerBase
{
    [Header("UI ELEMENTS")]
    [Space(20)]
    [SerializeField] private CircleFillHandler _progressAnswers;
    [SerializeField] private CircleFillHandler _progressScore;



    private ViewManager _viewManager;
    private bool _isWinner;



    void Awake()
    {
        _viewManager = FindObjectOfType<ViewManager>();
    }


    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        /// Don't forget to call the BASE at the init of Enter method
        base.Enter(state, runningState, callback);

        StartCoroutine(OpenFinalScore());
    }


    public override IEnumerator Exit()
    {
        if (_isWinner) animationsController.Tween_PlayByName("[EXIT WINNER]");
        else animationsController.Tween_PlayByName("[EXIT LOOSER]");

        /// Don't forget to call the BASE at the end of Exit coroutine
        return base.Exit();
    }



    private IEnumerator OpenFinalScore()
    {
        animationsController.Tween_PlayByName("[ENTER]");

        GameManager.PlayerStats thisPlayerStats = new GameManager.PlayerStats(siblingIndex);

        /// Enter progress radials
        _progressAnswers.MaxValue = thisPlayerStats.totalQuestions;
        StartCoroutine(_progressAnswers.SetValue(thisPlayerStats.rightQuestions));
        _progressAnswers.anim.Tween_PlayByName("[ENTER]");
        yield return new WaitForSeconds(0.3f);

        _progressScore.MaxValue = 100;
        StartCoroutine(_progressScore.SetValue(thisPlayerStats.score));
        _progressScore.anim.Tween_PlayByName("[ENTER]");


        /// Exit progress
        yield return new WaitForSeconds(5f);
        _progressAnswers.anim.Tween_PlayByName("[EXIT]");
        _progressScore.anim.Tween_PlayByName("[EXIT]");
        yield return new WaitForSeconds(0.3f);


        _isWinner = false;

        if (GameManager.gameSessionData.numberOfPlayersRunning == 1)
        {
            if (thisPlayerStats.score > 50f) _isWinner = true;
        }
        else if (GameManager.gameSessionData.numberOfPlayersRunning == 2)
        {
            int otherPlayerId = siblingIndex == 0 ? 1 : 0;
            GameManager.PlayerStats otherPlayerStats = new GameManager.PlayerStats(otherPlayerId);

            if (thisPlayerStats.score > otherPlayerStats.score) _isWinner = true;
        }


        if (_isWinner) animationsController.Tween_PlayByName("[ENTER WINNER]");
        else animationsController.Tween_PlayByName("[ENTER LOOSER]");


        ///
        /// ////////////////////////////////////////

        // /// Wait before to close and return to the Start page
        // yield return new WaitForSeconds(20f);

        // _playManager.Set_IDLE();
    }
}
