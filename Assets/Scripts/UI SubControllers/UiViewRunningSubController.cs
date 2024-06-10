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

                // StartCoroutine(OpenChapter(callback));
                print(gameObject.name + " - Open Chapter");
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
}
