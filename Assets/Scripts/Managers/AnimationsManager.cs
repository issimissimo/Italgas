using System.Collections;
using UnityEngine;
using System;


public class AnimationsManager : MonoBehaviour
{
    public static AnimationsManager instance;
    private UiAnimatedElement[] _animations;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }


    public void ExitAllAnimations()
    {
        _animations = FindObjectsOfType<UiAnimatedElement>();
        foreach (var a in _animations) a.Exit();
    }


    /// <summary>
    /// Close ALL animations (EXIT)
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitExitAllAnimations()
    {
        ExitAllAnimations();

        while (IsAnyAnimationPlaying(_animations))
            yield return null;
    }


    // /// <summary>
    // /// Close animation (EXIT)
    // /// </summary>
    // /// <returns></returns>
    // public IEnumerator WaitExitAnimation(UiAnimatedElement animation)
    // {
    //     _animations = new UiAnimatedElement[] { animation };

    //     ExitAllAnimations();

    //     while (IsAnyAnimationPlaying(_animations))
    //         yield return null;
    // }


    public IEnumerator WaitAnimation(UiAnimatedElementBase animation)
    {
        while (!animation.IsOnEmptyState())
            yield return null;
        
    }




    /// <summary>
    /// Check ANY "UiAnimatedElement" of the array is EMPTY state
    /// </summary>
    /// <param name="animations"></param>
    /// <returns></returns> <summary>
    public bool IsAnyAnimationPlaying(UiAnimatedElement[] animations)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsOnEmptyState() == false);
        return firstMatch == null ? false : true;
    }
}
