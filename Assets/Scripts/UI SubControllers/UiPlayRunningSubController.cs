using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack;
using UnityExtensions.Tween;

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
    // private List<UiAnimatedElement> _answerListAnimations = new List<UiAnimatedElement>();
    // private List<TweenPlayer> _answerAnimations = new List<TweenPlayer>();
    private bool _isWaiting;



    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();

        // foreach (var a in _answerList) _answerListAnimations.Add(a.animatedElement);
        // foreach (var a in _answerList) _answerAnimations.Add(a.animatedElement);
        _pageCanvasGroup.interactable = false;

        _chapterNameText.text = "";
        _questionText.text = "";
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
        switch (_currentRunningState)
        {
            case UiController.RUNNING_STATE.CHAPTER:

                break;

            case UiController.RUNNING_STATE.PAGE:

                StartCoroutine(ClosePage());
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
        // animationsController.Animations_EnterByName("ChapterName");
        animationsController.Tween_PlayByName("[ENTER CHAPTER]");

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
        // animationsController.Animations_EnterByName("Question");
        animationsController.Tween_PlayByName("[ENTER QUESTION]");



        // yield break;

        /// Setup the Answer Buttons
        List<TweenPlayer> buttonsAnimation = new List<TweenPlayer>();

        for (int i = 0; i < _answerList.Length; i++)
        {
            AnswerButtonComponent answerBttn = _answerList[i];
            answerBttn.Setup(
                text: GameManager.currentGamePage.answers[i].title,
                result: GameManager.currentGamePage.answers[i].isTrue,
                number: i
            );
            answerBttn.button.onClick.RemoveAllListeners();
            answerBttn.button.onClick.AddListener(() => AnswerButtonListener(answerBttn));

            // answerBttn.animatedElement.Enter();
            StartCoroutine(answerBttn.animationsController.Tween_PlayByNameWithDelay("[ENTER]", 0.5f));

        }

        /// Let's wait for all animation ENTER
        // while (animationsController.Animations_IsAnyPlaying(_answerListAnimations.ToArray(), "Enter"))
        //     yield return null;
        yield return new WaitForSeconds(2); /// e vaffanculo!




        /// Setup the Countdown
        _countdownProgressBar.maxValue = GameManager.currentGameVersion.maxTimeInSeconds;
        yield return null;
        _countdownProgressBar.currentPercent = GameManager.currentGameVersion.maxTimeInSeconds - 0.1f; /// weird...
        // animationsController.Animations_EnterByName("Countdown");
        animationsController.Tween_PlayByName("[ENTER COUNTDOWN]");

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


        // animationsController.Animations_ExitByName("Countdown");

        int buttonPressed = _playManager.myPlayer.NetworkedButtonPressedNumber;

        // for (int i = 0; i < _answerListAnimations.Count; i++)
        // {
        //     if (i == buttonPressed) _answerListAnimations[i].PlayAnimation("Clicked");
        //     else _answerListAnimations[i].PlayAnimation("NotClicked");
        // }


        for (int i = 0; i < _answerList.Length; i++)
        {
            if (i == buttonPressed)
            {
                _answerList[i].animationsController.Tween_PlayByName("[CLICKED]");
            }
        }










        /// We must wait for animation finished,
        /// for aesthetic reasons, but mainly because we can't call
        /// immediately another state change
        yield return new WaitForSeconds(1f);

        /// callback is "myPlayer.Set_RUNNING_STATE_FINISHED"
        callback.Invoke();
    }



    private void ShowWaitOtherPlayer()
    {
        _isWaiting = true;
        // animationsController.Animations_EnterByName("WaitOtherPlayer");
        animationsController.Tween_PlayByName("[ENTER WAIT]");
    }



    /// <summary>
    /// CLOSE THE PAGE
    /// </summary>
    private IEnumerator ClosePage()
    {
        // animationsController.Animations_ExitByName("Question");
        

        if (_isWaiting)
        {
            _isWaiting = false;
            // animationsController.Animations_ExitByName("WaitOtherPlayer");
            animationsController.Tween_PlayByName("[EXIT WAIT]");
        }
        else
        {
            animationsController.Tween_PlayByName("[EXIT COUNTDOWN]");
        }



        int buttonPressed = _playManager.myPlayer.NetworkedButtonPressedNumber;
        bool isTrue = _playManager.myPlayer.NetworkedAnswerResult;


        // for (int i = 0; i < _answerListAnimations.Count; i++)
        // {
        //     if (i == buttonPressed)
        //     {
        //         if (isTrue)
        //         {
        //             _answerListAnimations[i].PlayAnimation("IsRight");
        //             _answerList[i].animationsController.Lottie_PlayByName("IsRight");
        //         }
        //         else
        //         {
        //             _answerListAnimations[i].PlayAnimation("IsWrong");
        //         }
        //     }
        //     else _answerListAnimations[i].Exit();
        // }

        for (int i = 0; i < _answerList.Length; i++)
        {
            if (i == buttonPressed)
            {
                if (isTrue)
                {
                    _answerList[i].animationsController.Tween_PlayByName("[BUTTON TRUE]");
                    // _answerList[i].animationsController.Lottie_PlayByName("IsRight");
                }
                else
                {
                    _answerList[i].animationsController.Tween_PlayByName("[BUTTON FALSE]");
                }
            }
            // else _answerListAnimations[i].Exit();
        }

        /// Let's wait a little...
        yield return new WaitForSeconds(3);

        animationsController.Tween_PlayByName("[EXIT QUESTION]");

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
