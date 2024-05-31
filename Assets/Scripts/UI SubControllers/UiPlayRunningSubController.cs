using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class UiPlayRunningSubController : GamePanelSubControllerBase
{
    [Header("NEW_CHAPTER")]
    [SerializeField] private TMP_Text _chapterNameText;

    [Header("NEW_PAGE")]
    [SerializeField] CanvasGroup _pageCanvasGroup;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private AnswerButtonComponent[] _answerList;

    [Header("FINALE_SCORE")]
    [SerializeField] private TMP_Text _totalTime;
    [SerializeField] private TMP_Text _rightAnswers;
    [SerializeField] private TMP_Text _winnerOrLooser;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private UiAnimatedElement _finalScoreAnimationCtrl;


    private PlayManager _playManager;
    private List<UiAnimatedElement> _answerListAnimations = new List<UiAnimatedElement>();


    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();
        foreach (var a in _answerList) _answerListAnimations.Add(a.animationController);
        _pageCanvasGroup.interactable = false;
    }


    public override void SetUI_on_RUNNING_STATE(UiControllerBase.RUNNING_STATE runningState, Action callback = null)
    {
        switch (runningState)
        {
            case UiControllerBase.RUNNING_STATE.OPEN_CHAPTER:

                OpenChapter();
                break;

            case UiControllerBase.RUNNING_STATE.OPEN_PAGE:

                StartCoroutine(OpenPage());
                break;

            case UiControllerBase.RUNNING_STATE.WAIT_OTHER_PLAYER:
                break;

            case UiControllerBase.RUNNING_STATE.CLOSE_PAGE:

                StartCoroutine(ClosePage(callback));
                break;

            case UiControllerBase.RUNNING_STATE.FINAL_SCORE:

                GameManager.PlayerStats myPlayerStats = new GameManager.PlayerStats(GameManager.userData.playerId);
                _totalTime.text = "Tempo totale: " + myPlayerStats.totalTime;
                _rightAnswers.text = "Risposte giuste: " + myPlayerStats.rightQuestions + "/" + myPlayerStats.totalQuestions;

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

                break;
        }
    }


    private void OpenChapter()
    {
        _chapterNameText.text = GameManager.currentGameChapter.chapterName;
    }


    /// <summary>
    /// OPEN THE PAGE
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenPage()
    {
        /// Setup the Question
        _questionText.text = GameManager.currentGamePage.question;

        /// Setup the Answer Buttons
        for (int i = 0; i < _answerList.Length; i++)
        {
            AnswerButtonComponent answerBttn = _answerList[i];
            answerBttn.Set(
                text: GameManager.currentGamePage.answers[i].title,
                result: GameManager.currentGamePage.answers[i].isTrue,
                number: i
            );
            answerBttn.button.onClick.RemoveAllListeners();
            answerBttn.button.onClick.AddListener(() => AnswerButtonClicked(answerBttn));

            answerBttn.animationController.Enter();
        }



        /// Let's wait for all animation ENTER
        while (AnimationsManager.instance.IsAnyAnimationPlaying(_answerListAnimations.ToArray(), "Enter"))
        {
            // print("SI APRONO I TASTIIIIIIIIIIII");
            yield return null;
        }


        _pageCanvasGroup.interactable = true;
    }


    /// <summary>
    /// CLOSE THE PAGE
    /// </summary>
    /// <param name="buttonPressed"></param>
    /// <param name="isTrue"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator ClosePage(Action callback)
    {

        int buttonPressed = _playManager._myPlayer.NetworkedButtonPressedNumber;
        bool isTrue = _playManager._myPlayer.NetworkedAnswerResult;

        print("------------> CLOSE PAGE!!! ");
        print("------------> buttonPressed: " + buttonPressed);
        print("------------> isTrue: " + isTrue);

        for (int i = 0; i < _answerListAnimations.Count; i++)
        {
            if (i == buttonPressed)
            {
                if (isTrue)
                {
                    // print(_answerListAnimations[i].gameObject.name + " ======> ExitTrue");
                    _answerListAnimations[i].ExitTrue();
                }
                
                else
                {
                    // print(_answerListAnimations[i].gameObject.name + " ======> ExitFalse");
                    _answerListAnimations[i].ExitFalse();
                }
                
            }
            else _answerListAnimations[i].Exit();
        }

        /// Let's wait for all animation EXIT
        while (AnimationsManager.instance.IsAnyAnimationNotInEmptyState(_answerListAnimations.ToArray()))
        {
            // print("SI CHIUDONO I TASTIIIIIIIIIIII");
            yield return null;
        }

        callback.Invoke();
    }



    /// <summary>
    /// BUTTON CLICKED LISTENER
    /// </summary>
    /// <param name="answerClicked"></param>
    private void AnswerButtonClicked(AnswerButtonComponent answerClicked)
    {
        // /// Avoid double click
        // if (_answerClicked == answerClicked) return;

        /// Avoid other unwanted clicks
        _pageCanvasGroup.interactable = false;

        // _answerClicked = answerClicked;

        GameManager.instance.StopTimer();

        /// Change animation state to CLICKED
        answerClicked.animationController.Clicked();

        // /// Change non clicked buttons animation state to EXIT
        // for (int i = 0; i < _answerList.Count; i++)
        // {
        //     if (_answerList[i] != answerClicked)
        //     {
        //         _answerList[i].animationController._exitDelay = i * 200;
        //         _answerList[i].animationController.Exit();
        //     }
        // }

        // /// Let's wait some time, to not change state immediately
        // /// when we are 1 player mode
        // await Task.Delay(1000);

        // /// Change animation value to IS_TRUE (or not) --> BUT does not change the STATE!!!!
        // /// We will EXIT it on next STATE change
        // answerClicked.animationController.SetIsTrue(answerClicked.isTrue);

        /// Send the information to the PlayManager that
        /// the answer has been clicked
        _playManager.OnAnswerButtonPressed(buttonNumber: answerClicked.buttonNumber, isTrue: answerClicked.isTrue);
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



    void Update()
    {
        _countdownText.text = (GameManager.instance.maximumSeconds - Mathf.Floor(GameManager.timer)).ToString();
    }


}
