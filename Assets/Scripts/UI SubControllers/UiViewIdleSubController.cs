using System.Collections;
using System;

public class UiViewIdleSubController : GamePanelSubControllerBase
{
    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        /// Don't forget to call the BASE at the init of Enter method
        base.Enter(state, runningState, callback);

        animationsController.Tween_PlayByName("[ENTER]");
    }


    public override IEnumerator Exit()
    {
        animationsController.Tween_PlayByName("[EXIT]");

        /// Don't forget to call the BASE at the end of Exit coroutine
        return base.Exit();
    }
}
