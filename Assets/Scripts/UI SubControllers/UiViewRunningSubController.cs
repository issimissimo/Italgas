using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiViewRunningSubController : GamePanelSubControllerBase
{
    private PlayManager _viewManager;

    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        /// Don't forget to call the BASE at the init of Enter method
        base.Enter(state, runningState, callback);
        
        switch (runningState)
        {
            case UiController.RUNNING_STATE.CHAPTER:

                StartCoroutine(OpenChapter(callback));
                break;

            case UiController.RUNNING_STATE.PAGE:

                OpenPage();
                break;

            case UiController.RUNNING_STATE.CLICKED:

                // StartCoroutine(OnAnswerClicked(callback));
                break;

            case UiController.RUNNING_STATE.WAITING:

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
    private IEnumerator OpenChapter(Action callback)
    {
        print(gameObject.name + " - OpenChapter - " + GameManager.currentGameChapter.chapterName);

        // _chapterNameText.text = GameManager.currentGameChapter.chapterName;

        // animationsController.Animations_EnterByName("ChapterName");
        // yield return null;
        // while (!animationsController.Animations_IsInEmptyState("ChapterName")) yield return null;
        // print("...CLOSE CHAPTER");

        yield return null;
        callback.Invoke();
    }



    private void OpenPage()
    {
        print(gameObject.name + " - OpenPage - ");
    }


}
