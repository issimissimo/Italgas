using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiViewIdleSubController : GamePanelSubControllerBase
{
    public override void Enter(UiController.STATE? state, UiController.RUNNING_STATE? runningState, Action callback)
    {
        /// Don't forget to call the BASE at the init of Enter method
        base.Enter(state, runningState, callback);

        
    }


    public override IEnumerator Exit()
    {


        /// Don't forget to call the BASE at the end of Exit coroutine
        return base.Exit();
    }
}
