using System.Collections;
using UnityEngine;
using System;

public class UiPlayIntroSubController : GamePanelSubControllerBase
{
    public override void SetupUI(UiController.STATE state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        // // if (!GameManager.instance.isDevelopment && GameManager.instance.isAppJustStarted)
        if (!GameManager.instance.isDevelopment && GameManager.instance.isAppJustStarted)
        {
            GameManager.instance.isAppJustStarted = false;
            StartCoroutine(ShowIntro(callback));
        }
        else callback.Invoke();
    }

    private IEnumerator ShowIntro(Action callback)
    {
        animationsController.Lottie_PlayByName("Intro");

        /// Wait for the length of the intro file
        yield return new WaitForSeconds(animationsController.Lottie_GetDuration_ByName("Intro"));

        callback.Invoke();
    }
}
