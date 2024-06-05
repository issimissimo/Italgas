using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UiAnimatedElement : UiAnimatedElementBase
{
    // public enum DOMAIN { STATE, RUNNING_STATE }
    // [SerializeField] DOMAIN _domain;
    // [SerializeField] UiController.STATE _STATE;
    // [SerializeField] UiController.RUNNING_STATE _RUNNING_STATE;

    // [SerializeField] bool _useAutoEnter = true;
    // [SerializeField] bool _useAutoExit = true;



    // private void OnEnable()
    // {
    //     UiController.UpdateAnimationsOnStateChange += StateChanged;
    //     UiController.UpdateAnimationsOnRunningStateChange += RunningStateChanged;
    // }

    // private void OnDisable()
    // {
    //     UiController.UpdateAnimationsOnStateChange -= StateChanged;
    //     UiController.UpdateAnimationsOnRunningStateChange -= RunningStateChanged;
    // }

    // private void StateChanged(UiController.STATE state)
    // {
    //     if (_domain == DOMAIN.STATE && state == _STATE)
    //     {
    //         if (!_isActivated && _useAutoEnter) Enter();
    //     }
    //     else
    //     {
    //         if (_isActivated && _useAutoExit) Exit();
    //     }
    // }


    // private void RunningStateChanged(UiController.RUNNING_STATE runningState)
    // {
    //     if (_domain == DOMAIN.RUNNING_STATE && runningState == _RUNNING_STATE)
    //     {
    //         if (!_isActivated && _useAutoEnter) Enter();
    //     }
    //     else
    //     {
    //         if (_isActivated && _useAutoExit) Exit();
    //     }
    // }

}
