using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiViewRunningSubController : GamePanelSubControllerBase
{
    private PlayManager _viewManager;
    
    public override void SetupUI(UiController.STATE state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        switch (runningState)
        {
            case UiController.RUNNING_STATE.OPEN_CHAPTER:

                StartCoroutine(OpenChapter(callback));
                break;

            case UiController.RUNNING_STATE.OPEN_PAGE:

                // StartCoroutine(OpenPage());
                break;

            case UiController.RUNNING_STATE.ANSWER_CLICKED:

                // StartCoroutine(OnAnswerClicked(callback));
                break;

            case UiController.RUNNING_STATE.WAIT_OTHER_PLAYER:

                // ShowWaitOtherPlayer();
                break;

            case UiController.RUNNING_STATE.CLOSE_PAGE:

                // StartCoroutine(ClosePage(callback));
                break;
        }
    }


     /// <summary>
    /// OPEN THE CHAPTER
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenChapter(Action callback)
    {
        print(gameObject.name + " - OpenChapter - " + GameManager.currentGameChapter.chapterName);
        yield return null;
        // _chapterNameText.text = GameManager.currentGameChapter.chapterName;

        // animationsController.Animations_EnterByName("ChapterName");
        // yield return null;
        // while (!animationsController.Animations_IsInEmptyState("ChapterName")) yield return null;
        // print("...CLOSE CHAPTER");
        // callback.Invoke();
    }
}
