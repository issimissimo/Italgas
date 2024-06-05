using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class UiAnimatedElement : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    public int _enterDelay = 0;
    public int _exitDelay = 0;
    public string Name { get; private set; }
    public bool isActivated { get; private set; }
    

    void Awake()
    {
        Name = _animator.runtimeAnimatorController.name;
    }

    public async void Enter()
    {
        if (isActivated) return;
        isActivated = true;
        await Task.Delay(_enterDelay);
        _animator.SetTrigger("ENTER");
    }

    public async void Exit()
    {
        if (!isActivated) return;
        isActivated = false;
        await Task.Delay(_exitDelay);
        _animator.SetTrigger("EXIT");
        // print(gameObject.name + " ===> EXIT");
    }

    public async void ExitTrue()
    {
        if (!isActivated) return;
        await Task.Delay(_exitDelay);
        isActivated = false;
        _animator.SetTrigger("EXIT_TRUE");
        // print(gameObject.name + " ===> EXIT_TRUE");
    }

    public async void ExitFalse()
    {
        if (!isActivated) return;
        await Task.Delay(_exitDelay);
        isActivated = false;
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

}
