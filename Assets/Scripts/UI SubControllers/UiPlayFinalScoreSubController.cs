using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UiPlayFinalScoreSubController : GamePanelSubControllerBase
{
    [Header("UI ELEMENTS")]
    [SerializeField] private TMP_Text _totalTime;
    [SerializeField] private TMP_Text _rightAnswers;
    [SerializeField] private TMP_Text _winnerOrLooser;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private UiAnimatedElement _finalScoreAnimationCtrl;

    private PlayManager _playManager;


    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();
    }


    public override void SetupUI(UiController.STATE state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        StartCoroutine(OpenFinalScore());
    }



    private IEnumerator OpenFinalScore()
    {
        Animations_EnterByName("FinalScore");
        
        
        GameManager.PlayerStats myPlayerStats = new GameManager.PlayerStats(GameManager.userData.playerId);
        _totalTime.text = "Tempo totale: " + myPlayerStats.totalTime;
        _rightAnswers.text = "Risposte giuste: " + myPlayerStats.rightQuestions + "/" + myPlayerStats.totalQuestions;

        print("Tempo totale: " + myPlayerStats.totalTime);
        print("Risposte giuste: " + myPlayerStats.rightQuestions + "/" + myPlayerStats.totalQuestions);

        if (GameManager.gameSessionData.numberOfPlayersRunning == 1)
        {
            _winnerOrLooser.gameObject.SetActive(false);
            _score.gameObject.SetActive(true);
            _score.text = "Punteggio: " + myPlayerStats.score.ToString();
        }
        else if (GameManager.gameSessionData.numberOfPlayersRunning == 2)
        {
            int otherPlayerId = GameManager.userData.playerId == 0 ? 1 : 0;
            GameManager.PlayerStats otherPlayerStats = new GameManager.PlayerStats(otherPlayerId);

            _winnerOrLooser.gameObject.SetActive(true);
            _score.gameObject.SetActive(false);
            _winnerOrLooser.text = myPlayerStats.score > otherPlayerStats.score ? "HAI VINTO!!" : "HAI PERSO...";
        }

        yield return new WaitForSeconds(7);

        _playManager.Set_IDLE();
    }


    /// <summary>
    /// BUTTON
    /// </summary>
    public void BUTTON_ReturnToReady()
    {
        print("CLICCATO SU BUTTON_ReturnToReady");

        _finalScoreAnimationCtrl.Exit();

        // await Task.Delay(2000);

        _playManager.Set_IDLE();
    }
}
