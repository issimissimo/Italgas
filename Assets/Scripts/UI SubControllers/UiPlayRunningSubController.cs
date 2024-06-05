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


    void Awake()
    {
        _playManager = FindObjectOfType<PlayManager>();

        foreach (var a in _answerList) _answerListAnimations.Add(a.animationController);
        _pageCanvasGroup.interactable = false;
        // _pageCanvasGroupCtrl.Toggle(false);
    }


    public override void SetupUI(UiController.STATE state, UiController.RUNNING_STATE runningState, Action callback)
    {
        switch (runningState)
        {
            case UiController.RUNNING_STATE.OPEN_CHAPTER:

                StartCoroutine(OpenChapter(callback));
                break;

            case UiController.RUNNING_STATE.OPEN_PAGE:

                StartCoroutine(OpenPage());
                break;

            case UiController.RUNNING_STATE.ANSWER_CLICKED:

                StartCoroutine(OnAnswerClicked(callback));
                break;

            case UiController.RUNNING_STATE.WAIT_OTHER_PLAYER:

                ShowWaitOtherPlayer();
                break;

            case UiController.RUNNING_STATE.CLOSE_PAGE:

                StartCoroutine(ClosePage(callback));
                break;
        }
    }


    /// <summary>
    /// OPEN THE CHAPTER
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenChapter(Action callback)
    {
        print("OPEN CHAPTER....");
        _chapterNameText.text = GameManager.currentGameChapter.chapterName;

        Animations_EnterByName("ChapterName");
        yield return null;
        while (!Animations_IsInEmptyState("ChapterName")) yield return null;
        print("...CLOSE CHAPTER");
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
        // _questionAnimation.Enter();
        Animations_EnterByName("Question");

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
        while ( Animations_IsAnyPlaying(_answerListAnimations.ToArray(), "Enter"))
            yield return null;

        /// Setup the Countdown
        _countdownProgressBar.maxValue = GameManager.currentGameVersion.maxTimeInSeconds;
        yield return null;
        _countdownProgressBar.currentPercent = GameManager.currentGameVersion.maxTimeInSeconds - 0.1f; /// weird...
        // _countdownAnimation.Enter();
        Animations_EnterByName("Countdown");

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
        // _countdownAnimation.Exit();
        Animations_ExitByName("Countdown");

        int buttonPressed = _playManager._myPlayer.NetworkedButtonPressedNumber;
        UiAnimatedElement buttonPressedAnimation = null;

        /// Show clicked or not clicked button animations
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
        // _waitOtherPlayerAnimation.Enter();
        Animations_EnterByName("WaitOtherPlayer");
    }



    /// <summary>
    /// CLOSE THE PAGE
    /// </summary>
    private IEnumerator ClosePage(Action callback)
    {
        Animations_ExitByName("Question");
        Animations_ExitByName("WaitOtherPlayer");
        
        
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


        // _questionAnimation.Exit();
        // _waitOtherPlayerAnimation.Exit();

        /// Let's wait for button animations EXIT
        while (Animations_IsAnyNotInEmptyState(_answerListAnimations.ToArray()))
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

        /// Send the information to the PlayManager that
        /// the answer has been clicked
        _playManager.OnAnswerButtonPressed(buttonNumber: answerClicked.buttonNumber, isTrue: answerClicked.isTrue, time: timer);
    }

   



    void Update()
    {
        _countdownProgressBar.currentPercent = GameManager.currentGameVersion.maxTimeInSeconds - timer;
    }


}
