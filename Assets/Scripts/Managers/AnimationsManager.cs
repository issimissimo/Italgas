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
    public IEnumerator ExitAllAnimationsAndWaitForFinish()
    {
        ExitAllAnimations();

        while (IsAnyAnimationNotInEmptyState(_animations))
            yield return null;
    }



    /// <summary>
    /// Check ANY "UiAnimatedElement" of the array is EMPTY state
    /// </summary>
    /// <param name="animations"></param>
    /// <returns></returns> <summary>
    public bool IsAnyAnimationNotInEmptyState(UiAnimatedElement[] animations)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsOnEmptyState() == false);
        return firstMatch == null ? false : true;
    }


    
    /// <summary>
    /// Check ANY "UiAnimatedElement" of the array is playing some state
    /// </summary>
    /// <param name="animations"></param>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public bool IsAnyAnimationPlaying(UiAnimatedElement[] animations, string stateName)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsPlaying(stateName) == true);
        return firstMatch == null ? true : false;
    }
    
 
}
