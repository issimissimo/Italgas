using System.Collections;
using System;

public class UiIntroSubController : GamePanelSubControllerBase
{
    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        if (!GameManager.instance.isDevelopment && GameManager.instance.isAppJustStarted)
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

}
