using System.Collections;
using UnityEngine;
using System;
using LottiePlugin.UI;

public class UiViewIntroSubController : GamePanelSubControllerBase
{
    public AnimatedImage anim;

    public override void SetupUI(UiController.STATE state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        // // if (!GameManager.instance.isDevelopment && GameManager.instance.isAppJustStarted)
        // if (GameManager.instance.isAppJustStarted)
        // {
        //     GameManager.instance.isAppJustStarted = false;
        //     StartCoroutine(ShowIntro(callback));
        // }
        // else callback.Invoke();
        // StartCoroutine(ShowIntro(callback));

        // if (anim != null)
            anim.Play();
    }

    private IEnumerator ShowIntro(Action callback)
    {
        animationsController.Lottie_PlayByName("Intro");

        /// Wait for the length of the intro file
        yield return new WaitForSeconds(animationsController.Lottie_GetDuration_ByName("Intro"));
        print("FINITO");

        callback.Invoke();
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         // Color c = new Color(1,1,1,0.5f);
    //         // mat.SetColor("_Color", c);

    //         anim.Play();
    //     }
    // }
}
