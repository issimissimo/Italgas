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
        _isActivated = true;
        await Task.Delay(_enterDelay);
        _animator.SetTrigger("ENTER");
    }

    public async void Exit()
    {
        if (!_isActivated) return;
        _isActivated = false;
        await Task.Delay(_exitDelay);
        _animator.SetTrigger("EXIT");
        print(gameObject.name + " ===> EXIT");
    }

    public async void ExitTrue()
    {
        if (!_isActivated) return;
        await Task.Delay(_exitDelay);
        _isActivated = false;
        _animator.SetTrigger("EXIT_TRUE");
        print(gameObject.name + " ===> EXIT_TRUE");
    }

    public async void ExitFalse()
    {
        if (!_isActivated) return;
        await Task.Delay(_exitDelay);
        _isActivated = false;
        _animator.SetTrigger("EXIT_FALSE");
        print(gameObject.name + " ===> EXIT_FALSE");
    }

    public void Clicked()
    {
        print(gameObject.name + " --> CLICKED");
        _animator.SetTrigger("CLICKED");
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

}
