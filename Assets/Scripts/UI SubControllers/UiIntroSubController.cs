using System.Collections;
using UnityEngine;
using System;

public class UiIntroSubController : GamePanelSubControllerBase
{
    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        // // if (!GameManager.instance.isDevelopment && GameManager.instance.isAppJustStarted)
        if (GameManager.instance.isAppJustStarted)
        {
            animationsController.Tween_PlayByName("[ENTER]", callback);
        }
        else callback.Invoke();
    }


    public override IEnumerator Exit()
    {
        /// We don't need to call the Base,
        /// because we don't have exit time for this!
        yield return null;
    }


    // private IEnumerator ShowIntro(Action callback)
    // {
    //     animationsController.Lottie_PlayByName("Intro");

    //     /// Wait for the length of the intro file
    //     yield return new WaitForSeconds(animationsController.Lottie_GetDuration_ByName("Intro"));

    //     GameManager.instance.isAppJustStarted = false;

    //     callback.Invoke();
    // }
}
