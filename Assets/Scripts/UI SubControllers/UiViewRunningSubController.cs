using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.IO;


public class UiViewRunningSubController : GamePanelSubControllerBase
{
    [Header("UI ELEMENTS")]
    [SerializeField] private TMP_Text _chapterNameText;
    [SerializeField] private RawImage _chapterBackgroundImage;


    private ViewManager _viewManager;

    void Awake()
    {
        _viewManager = FindObjectOfType<ViewManager>();

    }



    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        /// Don't forget to call the BASE at the init of Enter method
        base.Enter(state, runningState, callback);

        switch (runningState)
        {
            case UiController.RUNNING_STATE.CHAPTER:

                OpenChapter(callback);
                break;

            case UiController.RUNNING_STATE.PAGE:

                OpenPage();
                break;

            case UiController.RUNNING_STATE.CLICKED:

                _currentRunningState = UiController.RUNNING_STATE.PAGE;
                OnAnswerClicked();
                break;

            case UiController.RUNNING_STATE.WAITING:

                _currentRunningState = UiController.RUNNING_STATE.PAGE;
                // ShowWaitOtherPlayer();
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

                // ClosePage();
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
        print(gameObject.name + " - OpenChapter - " + GameManager.currentGameChapter.chapterName);

        // _chapterNameText.text = GameManager.currentGameChapter.chapterName;

        // animationsController.Animations_EnterByName("ChapterName");
        // yield return null;
        // while (!animationsController.Animations_IsInEmptyState("ChapterName")) yield return null;
        // print("...CLOSE CHAPTER");

        /// Load background Image
        if (!string.IsNullOrEmpty(GameManager.currentGameChapter.backgroundImageName))
        {
            string filePath = Path.Combine(Application.persistentDataPath, GameManager.currentGameChapter.backgroundImageName);
            FileDownloader fileDownloader = new FileDownloader();
            StartCoroutine(fileDownloader.LoadFileFromUrlToRawImage(filePath, _chapterBackgroundImage, (result) =>
            {
                if (result.state != FileDownloader.STATE.SUCCESS)
                {
                    GameManager.instance.ShowModal("ERRORE", "Non è stato possibile caricare il file a questo percorso: " + filePath, true, true);
                }
                else
                {

                }
            }));
        }





        callback.Invoke();
    }



    private void OpenPage()
    {
        print(gameObject.name + " - OpenPage - ");
    }



    /// <summary>
    /// SHOW THE BUTTONS CLICKED / NOT CLICKED
    /// </summary>
    private void OnAnswerClicked()
    {
        // /// Stop Countdown
        // StopTimer();
        // animationsController.Animations_ExitByName("Countdown");



        print("Il numero di players è " + _viewManager.players.Count);

        int buttonPressed = 999;
        foreach (var p in _viewManager.players)
        {
            if (p.NetworkedId == siblingIndex) buttonPressed = p.NetworkedButtonPressedNumber;
        }

        print("Il player con ID: " + siblingIndex + " ha premuto il tasto N." + buttonPressed);




        // /// Show clicked or not clicked button animations
        // for (int i = 0; i < _answerListAnimations.Count; i++)
        // {
        //     if (i == buttonPressed) _answerListAnimations[i].PlayAnimation("Clicked");
        //     else _answerListAnimations[i].PlayAnimation("NotClicked");
        // }

        // /// We must wait for animation finished,
        // /// for aesthetic reasons, but mainly because we can't call
        // /// immediately another state change
        // yield return new WaitForSeconds(1f);

        // /// callback is "myPlayer.Set_RUNNING_STATE_FINISHED"
        // callback.Invoke();




    }


}
