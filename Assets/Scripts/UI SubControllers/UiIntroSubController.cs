using System.Collections;
using System;

public class UiIntroSubController : GamePanelSubControllerBase
{
    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        if (!GameManager.instance.isDevelopment && GameManager.instance.isAppJustStarted)
        {
            GameManager.instance.isAppJustStarted = false;
            animationsController.Tween_PlayByName("[ENTER]", callback);
            animationsController.Audio_PlayByName("[ENTER]");
        }
        else callback.Invoke();
    }


    public override IEnumerator Exit()
    {
        /// We don't need to call the Base,
        /// because we don't have exit time for this!
        yield return null;
    }

}
