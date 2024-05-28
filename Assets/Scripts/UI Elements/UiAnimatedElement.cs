using UnityEngine;

public class UiAnimatedElement : UiAnimatedElementBase
{
    public enum DOMAIN { STATE, RUNNING_STATE }
    [SerializeField] DOMAIN _domain;
    [SerializeField] UiControllerBase.STATE _STATE;
    [SerializeField] UiControllerBase.RUNNING_STATE _RUNNING_STATE;

    [SerializeField] bool _useAutoEnter = true;
    [SerializeField] bool _useAutoExit = true;



    private void OnEnable()
    {
        UiControllerBase.UpdateAnimationsOnStateChange += StateChanged;
        UiControllerBase.UpdateAnimationsOnRunningStateChange += RunningStateChanged;
    }

    private void OnDisable()
    {
        UiControllerBase.UpdateAnimationsOnStateChange -= StateChanged;
        UiControllerBase.UpdateAnimationsOnRunningStateChange -= RunningStateChanged;
    }

    private void StateChanged(UiControllerBase.STATE state)
    {
        if (_domain == DOMAIN.STATE && state == _STATE)
        {
            if (!_isActivated && _useAutoEnter) Enter();
        }
        else
        {
            if (_isActivated && _useAutoExit) Exit();
        }
    }


    private void RunningStateChanged(UiControllerBase.RUNNING_STATE runningState)
    {
        if (_domain == DOMAIN.RUNNING_STATE && runningState == _RUNNING_STATE)
        {
            if (!_isActivated && _useAutoEnter) Enter();
        }
        else
        {
            if (_isActivated && _useAutoExit) Exit();
        }
    }

}
