using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class UiAnimatedElement : MonoBehaviour
{
    private Animator _animator;
    public float _enterDelay = 0;
    public float _exitDelay = 0;
    public string animatorName { get; private set; }

    private Coroutine _playAnimationCoroutine;


    void Awake()
    {
        _animator = GetComponent<Animator>();
        animatorName = _animator.runtimeAnimatorController.name;
    }


    public void Enter(float? delay = null)
    {
        PlayAnimation("Enter", delay != null ? delay.Value : _enterDelay);
    }

    public void Exit(float? delay = null)
    {
        PlayAnimation("Exit", delay != null ? delay.Value : _exitDelay);
    }

    // public async void ExitTrue()
    // {
    //     await Task.Delay(_exitDelay);
    //     _animator.SetTrigger("EXIT_TRUE");
    //     // print(gameObject.name + " ===> EXIT_TRUE");
    // }

    // public async void ExitFalse()
    // {
    //     await Task.Delay(_exitDelay);
    //     _animator.SetTrigger("EXIT_FALSE");
    //     // print(gameObject.name + " ===> EXIT_FALSE");
    // }

    // public void Clicked()
    // {
    //     // print(gameObject.name + " --> CLICKED");
    //     _animator.SetTrigger("CLICKED");
    // }

    // public void NotClicked()
    // {
    //     _animator.SetTrigger("NOT_CLICKED");
    // }


    // public bool IsOnEmptyState()
    // {
    //     AnimatorStateInfo animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
    //     var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
    //     if (clipInfo.Length > 0)
    //         return clipInfo[0].clip.name == "Empty" ? true : false;
    //     else return true;
    // }

    public bool IsPlaying(string stateName)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else return false;
    }

    public void PlayAnimation(string stateName, float delay = 0f)
    {
        int stateID = Animator.StringToHash(stateName);
        if (!_animator.HasState(0, stateID))
        {
            Debug.LogError("The animation " + stateName + " don't exist!");
            return;
        }
        if (_playAnimationCoroutine != null) StopCoroutine(_playAnimationCoroutine);
        _playAnimationCoroutine = StartCoroutine(PlayAnimationCoroutine(stateName, delay));
    }

    private IEnumerator PlayAnimationCoroutine(string stateName, float delayTime)
    {
        print(gameObject.name + " - PlayAnimationCoroutine: " + stateName);
        yield return new WaitForSeconds(delayTime);
        print(gameObject.name + " - ORA SIIIIIIIIIIIIIII");
        _animator.Play(stateName);
        _playAnimationCoroutine = null;
    }

    // public float GetRunningAnimationTime()
    // {
    //     var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
    //     return clipInfo[0].clip.length;
    // }

    // public string GetRunningAnimationName()
    // {
    //     var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
    //     return clipInfo[0].clip.name;
    // }

}
