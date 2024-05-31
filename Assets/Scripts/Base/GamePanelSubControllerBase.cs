using UnityEngine;
using System;

public abstract class GamePanelSubControllerBase : MonoBehaviour
{
    public UiControllerBase.STATE behaviour;

    public virtual void SetUI_on_STATE()
    {
        /// To override
    }
    public virtual void SetUI_on_RUNNING_STATE(UiControllerBase.RUNNING_STATE runningState, Action callback = null)
    {
        /// To override
    }
}
