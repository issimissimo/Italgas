using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class UiPlayRunningSubController : GamePanelSubControllerBase
{
    [Header("NEW_CHAPTER")]
    [SerializeField] private TMP_Text _chapterNameText;
    [SerializeField] private UiAnimatedElement _chapterAnimation;

    [Header("NEW_PAGE")]
    [SerializeField] CanvasGroup _pageCanvasGroup;
    // [SerializeField] CanvasController _pageCanvasGroupCtrl;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private AnswerButtonComponent[] _answerList;
    [SerializeField] private UiAnimatedElement _questionAnimation;
    [SerializeField] private UiAnimatedElement _countdownAnimation;
    [SerializeField] private UiAnimatedElement _waitOtherPlayerAnimation;


    [Header("FINAL_SCORE")]
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
        // _pageCanvasGroupCtrl.Toggle(false);
    }


    public override void SetUI_on_RUNNING_STATE(UiControllerBase.RUNNING_STATE runningState, Action callback = null)
    {
        switch (runningState)
        {
            case UiControllerBase.RUNNING_STATE.OPEN_CHAPTER:

                StartCoroutine(OpenChapter(callback));
                break;

            case UiControllerBase.RUNNING_STATE.OPEN_PAGE:

                StartCoroutine(OpenPage());
                break;

            case UiControllerBase.RUNNING_STATE.ANSWER_CLICKED:

                StartCoroutine(OnAnswerClicked(callback));
                break;

            case UiControllerBase.RUNNING_STATE.WAIT_OTHER_PLAYER:

                ShowWaitOtherPlayer();
                break;

            case UiControllerBase.RUNNING_STATE.CLOSE_PAGE:

                StartCoroutine(ClosePage(callback));
                break;

            case UiControllerBase.RUNNING_STATE.FINAL_SCORE:

                OpenFinalScore();
                break;
        }
    }


    private IEnumerator OpenChapter(Action callback)
    {
        /// Setup the Chapter
        _chapterNameText.text = GameManager.currentGameChapter.chapterName;

        // print("APRO CHAPTER: " + GameManager.currentGameChapter.chapterName);

        /// Play the "Enter" animation
        _chapterAnimation.Enter();
        yield return null;

        /// Wait for animation finished
        while (!_chapterAnimation.IsOnEmptyState()) yield return null;

        // print("FINITO CHAPTER!");

        // print("CHIUDO CHAPTER");

        /// Callback
        callback.Invoke();
    }


    /// <summary>
    /// OPEN THE PAGE
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenPage()
    {
        /// Setup the Question
        _questionText.text = GameManager.currentGamePage.question;
        _questionAnimation.Enter();

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
            answerBttn.button.onClick.AddListener(() => AnswerButtonListener(answerBttn));

            answerBttn.animationController.Enter();
        }

        /// Let's wait for all animation ENTER
        while (AnimationsManager.instance.IsAnyAnimationPlaying(_answerListAnimations.ToArray(), "Enter"))
            yield return null;

        /// Setup the Countdown
        _countdownText.text = GameManager.instance.maximumSeconds.ToString();
        _countdownAnimation.Enter();

        /// Start the Countdown
        StartTimer(
            seconds: GameManager.instance.maximumSeconds,
            callback: () =>
                {
                    /// Time is finished!
                    print("TEMPO SCADUTO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    _playManager.OnAnswerButtonPressed(buttonNumber: -1, isTrue: false, time: timer);
                }
            );


        _pageCanvasGroup.interactable = true;
        // _pageCanvasGroupCtrl.Toggle(true);
    }



    /// <summary>
    /// SHOW THE BUTTONS CLICKED / NOT CLICKED
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator OnAnswerClicked(Action callback)
    {
        /// Stop Countdown
        StopTimer();
        _countdownAnimation.Exit();

        int buttonPressed = _playManager._myPlayer.NetworkedButtonPressedNumber;
        UiAnimatedElement buttonPressedAnimation = null;

        /// Show button animations
        for (int i = 0; i < _answerListAnimations.Count; i++)
        {
            if (i == buttonPressed)
            {
                _answerListAnimations[i].Clicked();
                buttonPressedAnimation = _answerListAnimations[i];
            }
            else _answerListAnimations[i].NotClicked();
        }

        /// Wait for animation finished
        yield return null;
        if (buttonPressedAnimation != null)
            while (buttonPressedAnimation.IsPlaying("Clicked")) yield return null;
        else yield return new WaitForSeconds(1);

        /// Callback
        callback.Invoke();
    }



    private void ShowWaitOtherPlayer()
    {
        _waitOtherPlayerAnimation.Enter();
    }



    /// <summary>
    /// CLOSE THE PAGE
    /// </summary>
    /// <param name="buttonPressed"></param>
    /// <param name="isTrue"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator ClosePage(Action callback)
    {

        int buttonPressed = _playManager._myPlayer.NetworkedButtonPressedNumber;
        bool isTrue = _playManager._myPlayer.NetworkedAnswerResult;

        print("------------> CLOSE PAGE!!! ");
        print("------------> buttonPressed: " + buttonPressed);
        print("------------> isTrue: " + isTrue);

        for (int i = 0; i < _answerListAnimations.Count; i++)
        {
            print("(((((((((((((((  TASTO N. " + i + " )))))))))))))))");
            if (i == buttonPressed)
            {
                if (isTrue)
                {
                    print(_answerListAnimations[i].gameObject.name + " QUESTO E' CLICCATO ED ESCE TRUE");
                    _answerListAnimations[i].ExitTrue();
                }

                else
                {
                    print(_answerListAnimations[i].gameObject.name + " QUESTO E' CLICCATO ED ESCE FALSE");
                    _answerListAnimations[i].ExitFalse();
                }

            }

            else
            {
                print(_answerListAnimations[i].gameObject.name + " QUESTO NON E' CLICCATO ED ESCE E BASTA");
                _answerListAnimations[i].Exit();

            }


        }

        
        _questionAnimation.Exit();
        _waitOtherPlayerAnimation.Exit();

        /// Let's wait for button animations EXIT
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
    private void AnswerButtonListener(AnswerButtonComponent answerClicked)
    {
        /// Avoid other unwanted clicks
        _pageCanvasGroup.interactable = false;
        // _pageCanvasGroupCtrl.Toggle(false);

        // /// Stop Countdown
        // GameManager.instance.StopTimer();

        // /// Change animation state to CLICKED
        // answerClicked.animationController.Clicked();

        /// Send the information to the PlayManager that
        /// the answer has been clicked
        _playManager.OnAnswerButtonPressed(buttonNumber: answerClicked.buttonNumber, isTrue: answerClicked.isTrue, time: timer);
    }



    private void OpenFinalScore()
    {
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
        _countdownText.text = (GameManager.instance.maximumSeconds - Mathf.Floor(timer)).ToString();
    }


}
