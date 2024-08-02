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
    [SerializeField] GameObject _chapter;
    [SerializeField] GameObject _page;
    [SerializeField] CanvasGroup _pageCanvasGroup;
    [SerializeField] private ProgressBar _countdownProgressBar;
    [SerializeField] private TMP_Text _questionText;

    [Space]
    [SerializeField] private AnswerButtonComponent[] _answerList;


    private PlayManager _playManager;
    private bool _isWaiting;



    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();
        _pageCanvasGroup.interactable = false;
        _questionText.gameObject.SetActive(false); /// used for text animator

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

                _chapter.SetActive(true);
                _page.SetActive(false);
                StartCoroutine(OpenChapter(callback));
                break;

            case UiController.RUNNING_STATE.PAGE:

                _chapter.SetActive(false);
                _page.SetActive(true);
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
    private IEnumerator OpenChapter(Action callback)
    {
        print("APRO CAPITOLO: " + GameManager.currentGameChapterIndex);
        _chapterNameText.text = "<wave a=0.2>" + GameManager.currentGameChapter.chapterName + "</wave>";
        animationsController.Tween_PlayByName("[ENTER CHAPTER]");

        yield return new WaitForSeconds(5); /// è il tempo dell'animazione di AE

        animationsController.Tween_PlayByName("[EXIT CHAPTER]");

        callback.Invoke();
    }


    /// <summary>
    /// OPEN THE PAGE
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenPage()
    {
        print("APRO PAGINA: " + GameManager.currentGamePageIndex + " DI: " + GameManager.currentGameChapter.pages.Count + " TEMPO: " + GameManager.currentGameVersion.maxTimeInSeconds);
        /// Check if page data are valid
        if (string.IsNullOrEmpty(GameManager.currentGamePage.question) || GameManager.currentGamePage.answers.Count < 4)
        {
            GameManager.instance.ShowModal("ERRORE!", "Non sono stati inseriti i dati per questa pagina!", true, false);
            yield break;
        }

        /// Setup the Question
        _questionText.text = GameManager.currentGamePage.question;
        animationsController.Tween_PlayByName("[ENTER QUESTION]");
        animationsController.Tween_PlayByName("[ENTER QUESTION MARK]");

        yield return new WaitForSeconds(1);


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

            StartCoroutine(answerBttn.animationsController.Tween_PlayByNameWithDelay("[ENTER]", 0.2f * (i + 1)));
            // StartCoroutine(answerBttn.animationsController.Audio_PlayByNameWithDelay("[ENTER]", 0.2f * (i + 1)));
        }

        /// Let's wait for all animation ENTER
        yield return new WaitForSeconds(1); /// e vaffanculo!

        /// Setup the Countdown
        _countdownProgressBar.maxValue = GameManager.currentGameVersion.maxTimeInSeconds;
        yield return null;
        _countdownProgressBar.currentPercent = GameManager.currentGameVersion.maxTimeInSeconds - 0.1f; /// weird...
        animationsController.Tween_PlayByName("[ENTER COUNTDOWN]");

        /// Start the Countdown
        StartTimer(
            seconds: GameManager.currentGameVersion.maxTimeInSeconds,
            callback: () =>
                {
                    /// Time is finished!
                    print("TEMPO SCADUTO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    animationsController.Audio_PlayByName("[TIME FINISHED]");
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
        /// Close Countdown
        StopTimer();
        animationsController.Tween_PlayByName("[EXIT COUNTDOWN]");

        /// Close the Question icon
        animationsController.Tween_PlayByName("[EXIT QUESTION MARK]");

        /// Show the clicked animation
        int buttonPressed = _playManager.myPlayer.NetworkedButtonPressedNumber;
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

        /// (callback is "myPlayer.Set_RUNNING_STATE_FINISHED")
        callback.Invoke();
    }



    private void ShowWaitOtherPlayer()
    {
        _isWaiting = true;
        animationsController.Tween_PlayByName("[ENTER WAIT]");
    }



    /// <summary>
    /// CLOSE THE PAGE
    /// </summary>
    private IEnumerator ClosePage()
    {
        if (_isWaiting)
        {
            _isWaiting = false;
            animationsController.Tween_PlayByName("[EXIT WAIT]");
        }
        
        yield return new WaitForSeconds(0.5f);

        /// Show if the pressed button is true or not
        int buttonPressed = _playManager.myPlayer.NetworkedButtonPressedNumber;
        bool isTrue = _playManager.myPlayer.NetworkedAnswerResult;
        bool isRightAnswer = false;
        for (int i = 0; i < _answerList.Length; i++)
        {
            if (i == buttonPressed)
            {
                if (isTrue)
                {
                    _answerList[i].animationsController.Tween_PlayByName("[BUTTON TRUE]");
                    isRightAnswer = true;
                }
                else
                {
                    _answerList[i].animationsController.Tween_PlayByName("[BUTTON FALSE]");
                }
            }
        }


        if (!isRightAnswer)
        {
            /// Show the "Wrong" Icon
            animationsController.Tween_PlayByName("[ENTER FINGER DOWN]");

            /// Show the right answer button if the
            /// pressed button was false, or no button has been pressed
            foreach (var a in _answerList)
            {
                if (a.isTrue)
                {
                    a.animationsController.Tween_PlayByName("[BUTTON WAS TRUE]");
                    a.answerText.text = "<color=green><bounce a=0.2>" + a.answerText.text + "</bounce>";
                }
            }
        }
        else
        {
            /// Show the "Right" Icon
            animationsController.Tween_PlayByName("[ENTER FINGER UP]");
        }


        /// Let's wait a little...
        yield return new WaitForSeconds(6);

        /// Close all
        animationsController.Tween_PlayByName("[EXIT QUESTION]");

        if (!isRightAnswer) animationsController.Tween_PlayByName("[EXIT FINGER DOWN]");
        else animationsController.Tween_PlayByName("[EXIT FINGER UP]");

        foreach (var a in _answerList)
            a.animationsController.Tween_PlayByName("[EXIT]");

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
