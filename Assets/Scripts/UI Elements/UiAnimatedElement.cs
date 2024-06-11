using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class UiAnimatedElement : MonoBehaviour
{
    private Animator _animator;
    // {
    //     get
    //     {
    //         if (__animator == null) __animator = 
    //         return __animator;
    //     }
    //     set { __animator = value; }
    // }
    private Animator __animator;
    public int _enterDelay = 0;
    public int _exitDelay = 0;
    public string animatorName { get; private set; }
    public bool isActivated { get; private set; }
    


    void Awake()
    {
        _animator = GetComponent<Animator>();
        animatorName = _animator.runtimeAnimatorController.name;
    }


    public async void Enter()
    {
        await Task.Delay(_enterDelay);
        _animator.SetTrigger("ENTER");
    }

    public async void Exit()
    {
        await Task.Delay(_exitDelay);
        _animator.SetTrigger("EXIT");
        // print(gameObject.name + " ===> EXIT");
    }

    public async void ExitTrue()
    {
        await Task.Delay(_exitDelay);
        _animator.SetTrigger("EXIT_TRUE");
        // print(gameObject.name + " ===> EXIT_TRUE");
    }

    public async void ExitFalse()
    {
        await Task.Delay(_exitDelay);
        _animator.SetTrigger("EXIT_FALSE");
        // print(gameObject.name + " ===> EXIT_FALSE");
    }

    public void Clicked()
    {
        // print(gameObject.name + " --> CLICKED");
        _animator.SetTrigger("CLICKED");
    }

    public void NotClicked()
    {
        _animator.SetTrigger("NOT_CLICKED");
    }


    public bool IsOnEmptyState()
    {
        AnimatorStateInfo animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
            return clipInfo[0].clip.name == "Empty" ? true : false;
        else return true;
    }

    public bool IsPlaying(string stateName)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else return false;
    }

    public float GetRunningAnimationTime()
    {
        var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        return clipInfo[0].clip.length;
    }

    public string GetRunningAnimationName()
    {
        var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        return clipInfo[0].clip.name;
    }

}
