using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack;

public class UiPlayRunningSubController : GamePanelSubControllerBase
{
    [Header("UI ELEMENTS")]
    [SerializeField] private TMP_Text _chapterNameText;

    [Space]
    [SerializeField] CanvasGroup _pageCanvasGroup;
    [SerializeField] private ProgressBar _countdownProgressBar;
    [SerializeField] private TMP_Text _questionText;

    [Space]
    [SerializeField] private AnswerButtonComponent[] _answerList;


    private PlayManager _playManager;
    private List<UiAnimatedElement> _answerListAnimations = new List<UiAnimatedElement>();
    private bool _isWaiting;



    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();

        foreach (var a in _answerList) _answerListAnimations.Add(a.animatedElement);
        // _pageCanvasGroup.interactable = false;
    }


    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        /// Don't forget to call the BASE at the init of Enter method
        base.Enter(state, runningState, callback);

        _pageCanvasGroup.interactable = false;

        switch (runningState)
        {
            case UiController.RUNNING_STATE.CHAPTER:

                OpenChapter(callback);
                break;

            case UiController.RUNNING_STATE.PAGE:

                StartCoroutine(OpenPage());
                break;

            case UiController.RUNNING_STATE.CLICKED:

                _currentRunningState = UiController.RUNNING_STATE.PAGE;
                StartCoroutine(OnAnswerClicked(callback));
                break;

            case UiController.RUNNING_STATE.WAITING:

                _currentRunningState = UiController.RUNNING_STATE.PAGE;
                ShowWaitOtherPlayer();
                break;
        }
    }


    public override IEnumerator Exit()
    {
        // StartCoroutine(animationsController.CloseAll());
        switch (_currentRunningState)
        {
            case UiController.RUNNING_STATE.CHAPTER:

                break;

            case UiController.RUNNING_STATE.PAGE:

                ClosePage();
                break;

            case UiController.RUNNING_STATE.CLICKED:

                print("ESCO DA 'ANSWER_CLICKED'");
                break;

            case UiController.RUNNING_STATE.WAITING:

                print("ESCO DA 'WAIT_OTHER_PLAYER'");
                break;
        }

        /// Don't forget to call the BASE at the end of Exit coroutine
        return base.Exit();
    }


    /// <summary>
    /// OPEN THE CHAPTER
    /// </summary>
    /// <returns></returns>
    private void OpenChapter(Action callback)
    {
        print("OPEN CHAPTER.... " + GameManager.currentGameChapter.chapterName);
        _chapterNameText.text = GameManager.currentGameChapter.chapterName;
        animationsController.Animations_EnterByName("ChapterName");

        callback.Invoke();
    }


    /// <summary>
    /// OPEN THE PAGE
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenPage()
    {
        /// Check if page data are valid
        if (string.IsNullOrEmpty(GameManager.currentGamePage.question) || GameManager.currentGamePage.answers.Count < 4)
        {
            GameManager.instance.ShowModal("ERRORE!", "Non sono stati inseriti i dati per questa pagina!", true, false);
            yield break;
        }

        /// Setup the Question
        _questionText.text = GameManager.currentGamePage.question;
        animationsController.Animations_EnterByName("Question");

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

            answerBttn.animatedElement.Enter();
        }

        /// Let's wait for all animation ENTER
        while (animationsController.Animations_IsAnyPlaying(_answerListAnimations.ToArray(), "Enter"))
            yield return null;

        /// Setup the Countdown
        _countdownProgressBar.maxValue = GameManager.currentGameVersion.maxTimeInSeconds;
        yield return null;
        _countdownProgressBar.currentPercent = GameManager.currentGameVersion.maxTimeInSeconds - 0.1f; /// weird...
        animationsController.Animations_EnterByName("Countdown");

        /// Start the Countdown
        StartTimer(
            seconds: GameManager.currentGameVersion.maxTimeInSeconds,
            callback: () =>
                {
                    /// Time is finished!
                    print("TEMPO SCADUTO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    _playManager.OnAnswerButtonPressed(buttonNumber: -1, isTrue: false, time: timer);
                }
            );


        _pageCanvasGroup.interactable = true;
    }



    /// <summary>
    /// SHOW THE BUTTONS CLICKED / NOT CLICKED
    /// </summary>
    private IEnumerator OnAnswerClicked(Action callback)
    {
        /// Stop Countdown
        StopTimer();
        animationsController.Animations_ExitByName("Countdown");

        int buttonPressed = _playManager.myPlayer.NetworkedButtonPressedNumber;

        /// Show clicked or not clicked button animations
        for (int i = 0; i < _answerListAnimations.Count; i++)
        {
            if (i == buttonPressed) _answerListAnimations[i].PlayAnimation("Clicked");
            else _answerListAnimations[i].PlayAnimation("NotClicked");
        }

        /// We must wait for animation finished,
        /// for aesthetic reasons, but mainly because we can't call
        /// immediately another state change
        yield return new WaitForSeconds(2f);

        /// callback is "myPlayer.Set_RUNNING_STATE_FINISHED"
        callback.Invoke();
    }



    private void ShowWaitOtherPlayer()
    {
        animationsController.Animations_EnterByName("WaitOtherPlayer");
        _isWaiting = true;
    }



    /// <summary>
    /// CLOSE THE PAGE
    /// </summary>
    private void ClosePage()
    {
        animationsController.Animations_ExitByName("Question");

        if (_isWaiting)
        {
            animationsController.Animations_ExitByName("WaitOtherPlayer");
            _isWaiting = false;
        }



        int buttonPressed = _playManager.myPlayer.NetworkedButtonPressedNumber;
        bool isTrue = _playManager.myPlayer.NetworkedAnswerResult;


        for (int i = 0; i < _answerListAnimations.Count; i++)
        {
            if (i == buttonPressed)
            {
                if (isTrue)
                {
                    _answerListAnimations[i].PlayAnimation("IsRight");
                    _answerList[i].animationsController.Lottie_PlayByName("IsRight");
                }
                else
                {
                    _answerListAnimations[i].PlayAnimation("IsWrong");
                }
            }
            else _answerListAnimations[i].Exit();
        }


        // /// Let's wait for button animations EXIT
        // while (animationsController.Animations_IsAnyNotInEmptyState(_answerListAnimations.ToArray()))
        //     yield return null;

        // callback.Invoke();
    }



    /// <summary>
    /// BUTTON CLICKED LISTENER
    /// </summary>
    /// <param name="answerClicked"></param>
    private void AnswerButtonListener(AnswerButtonComponent answerClicked)
    {
        /// Avoid other unwanted clicks
        _pageCanvasGroup.interactable = false;

        /// Send the information to the PlayManager that
        /// the answer has been clicked
        _playManager.OnAnswerButtonPressed(buttonNumber: answerClicked.buttonNumber, isTrue: answerClicked.isTrue, time: timer);
    }





    void Update()
    {
        _countdownProgressBar.currentPercent = GameManager.currentGameVersion.maxTimeInSeconds - timer;
    }


}
