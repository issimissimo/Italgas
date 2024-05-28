using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class UiAnimatedElementBase : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    public int _enterDelay = 0;
    public int _exitDelay = 0;


    public bool isAnimationFinished { get; private set; }
    protected bool _isActivated = false;



    public async void Enter()
    {
        if (_isActivated) return;
        await Task.Delay(_enterDelay);
        _isActivated = true;
        _animator.SetTrigger("ENTER");
    }

    public async void Exit()
    {
        if (!_isActivated) return;
        await Task.Delay(_exitDelay);
        _isActivated = false;
        _animator.SetTrigger("EXIT");
    }

    public void Clicked()
    {
        _animator.SetTrigger("CLICKED");
    }

    public void SetIsTrue(bool value)
    {
        _animator.SetInteger("IS_TRUE", value ? 1 : 0);
    }

    public void SetRESULT(bool value)
    {
        _animator.SetBool("RESULT", value);
    }


    public bool IsOnEmptyState()
    {
        AnimatorStateInfo animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        return clipInfo[0].clip.name == "Empty" ? true : false;
        else return true;
    }
}
