using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiPlayIntroSubController : GamePanelSubControllerBase
{
    public override void SetupUI(UiController.STATE state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        StartCoroutine(ShowIntro());
    }

    private IEnumerator ShowIntro()
    {
        // if (!GameManager.instance.isDevelopment && GameManager.instance.isAppJustStarted)
        if (GameManager.instance.isAppJustStarted)
        {
            GameManager.instance.isAppJustStarted = false;

            /// Show the intro
            /// 
            print("INTROOOOOOOOOOOOOOOOOOOOOOOOOOO");

            animationsController.Lottie_PlayByName("Intro");

            yield return new WaitForSeconds(5);
        }

        PlayerController[] players = FindObjectsOfType<PlayerController>();


    }
}
