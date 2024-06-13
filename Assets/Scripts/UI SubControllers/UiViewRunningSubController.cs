using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiViewRunningSubController : GamePanelSubControllerBase
{
    private PlayManager _viewManager;
    
    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        switch (runningState)
        {
            case UiController.RUNNING_STATE.CHAPTER:

                StartCoroutine(OpenChapter(callback));
                break;

            case UiController.RUNNING_STATE.PAGE:

                // StartCoroutine(OpenPage());
                break;

            case UiController.RUNNING_STATE.CLICKED:

                // StartCoroutine(OnAnswerClicked(callback));
                break;

            case UiController.RUNNING_STATE.WAITING:

                // ShowWaitOtherPlayer();
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
