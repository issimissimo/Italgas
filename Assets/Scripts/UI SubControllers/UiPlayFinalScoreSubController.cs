using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiPlayFinalScoreSubController : GamePanelSubControllerBase
{
    [Header("UI ELEMENTS")]
    // [SerializeField] private TMP_Text _totalTime;
    // [SerializeField] private TMP_Text _rightAnswers;
    // [SerializeField] private TMP_Text _winnerOrLooser;
    // [SerializeField] private TMP_Text _score;
    // [SerializeField] private UiAnimatedElement _finalScoreAnimationCtrl;

    [Space(20)]
    [SerializeField] private CircleFillHandler _progressAnswers;
    [SerializeField] private CircleFillHandler _progressScore;



    private PlayManager _playManager;
    private bool _isWinner;



    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();
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

        GameManager.PlayerStats myPlayerStats = new GameManager.PlayerStats(GameManager.userData.playerId);

        /// Enter progress radials
        _progressAnswers.MaxValue = myPlayerStats.totalQuestions;
        StartCoroutine(_progressAnswers.SetValue(myPlayerStats.rightQuestions));
        _progressAnswers.anim.Tween_PlayByName("[ENTER]");
        _progressAnswers.anim.Audio_PlayByName("[ENTER]");
        yield return new WaitForSeconds(0.3f);

        _progressScore.MaxValue = 100;
        StartCoroutine(_progressScore.SetValue(myPlayerStats.score));
        _progressScore.anim.Tween_PlayByName("[ENTER]");
        _progressScore.anim.Audio_PlayByName("[ENTER]");


        /// Exit progress
        yield return new WaitForSeconds(5f);
        _progressAnswers.anim.Tween_PlayByName("[EXIT]");
        _progressScore.anim.Tween_PlayByName("[EXIT]");
        yield return new WaitForSeconds(0.3f);


        _isWinner = false;

        if (GameManager.gameSessionData.numberOfPlayersRunning == 1)
        {
            if (myPlayerStats.score > 50f) _isWinner = true;
        }
        else if (GameManager.gameSessionData.numberOfPlayersRunning == 2)
        {
            int otherPlayerId = GameManager.userData.playerId == 0 ? 1 : 0;
            GameManager.PlayerStats otherPlayerStats = new GameManager.PlayerStats(otherPlayerId);

            if (myPlayerStats.score > otherPlayerStats.score) _isWinner = true;
        }


        if (_isWinner) animationsController.Tween_PlayByName("[ENTER WINNER]");
        else animationsController.Tween_PlayByName("[ENTER LOOSER]");


        yield return new WaitForSeconds(10f);
        _playManager.Set_IDLE();
    }





















    // animationsController.Animations_EnterByName("FinalScore");


    // GameManager.PlayerStats myPlayerStats = new GameManager.PlayerStats(GameManager.userData.playerId);
    // _totalTime.text = "Tempo totale: " + myPlayerStats.totalTime;
    // _rightAnswers.text = "Risposte giuste: " + myPlayerStats.rightQuestions + "/" + myPlayerStats.totalQuestions;

    // print("Tempo totale: " + myPlayerStats.totalTime);
    // print("Risposte giuste: " + myPlayerStats.rightQuestions + "/" + myPlayerStats.totalQuestions);

    // if (GameManager.gameSessionData.numberOfPlayersRunning == 1)
    // {
    //     _winnerOrLooser.gameObject.SetActive(false);
    //     _score.gameObject.SetActive(true);
    //     _score.text = "Punteggio: " + myPlayerStats.score.ToString();
    // }
    // else if (GameManager.gameSessionData.numberOfPlayersRunning == 2)
    // {
    //     int otherPlayerId = GameManager.userData.playerId == 0 ? 1 : 0;
    //     GameManager.PlayerStats otherPlayerStats = new GameManager.PlayerStats(otherPlayerId);

    //     _winnerOrLooser.gameObject.SetActive(true);
    //     _score.gameObject.SetActive(false);
    //     _winnerOrLooser.text = myPlayerStats.score > otherPlayerStats.score ? "HAI VINTO!!" : "HAI PERSO...";
    // }

    // yield return new WaitForSeconds(3);

    // _playManager.Set_IDLE();
}

