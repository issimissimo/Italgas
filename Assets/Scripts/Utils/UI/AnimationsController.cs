using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationsController : MonoBehaviour
{
    [Header("LOTTIE ANIMATIONS")]
    [SerializeField] protected LottieAnimation[] _lottieAnimations;

    [Header("STANDARD ANIMATIONS")]
    [SerializeField] protected UiAnimatedElement[] _standardAnimations;


    //#region COMMONS

    
    /// <summary>
    /// Close all animations and stop Lottie files at the end
    /// </summary>
    /// <returns></returns>
    public IEnumerator CloseAll()
    {
        /// Exit all animations
        yield return Animations_ExitAll();
        
        /// Stop all Lottie animations
        Lottie_StopAll();

        yield return null;
    }


    //#endregion




    //#region STANDARD ANIMATIONS MANAGER

    public void Animations_EnterAll()
    {
        foreach (var a in _standardAnimations) a.Enter();
    }

    public IEnumerator Animations_ExitAll()
    {
        foreach (var a in _standardAnimations) a.Exit();

        yield return null;

        while (Animations_IsAnyPlaying("Exit"))
            yield return null;
    }

    public void Animations_EnterByName(string name)
    {
        foreach (var anim in _standardAnimations) if (anim.animatorName == name) anim.Enter();
    }

    public void Animations_ExitByName(string name)
    {
        foreach (var anim in _standardAnimations) if (anim.animatorName == name) anim.Exit();
    }

    public UiAnimatedElement Animations_GetByName(string name)
    {
        foreach (var anim in _standardAnimations)
        {
            if (anim.animatorName == name) return anim;
        }
        return null;
    }

    // public bool Animations_IsInEmptyState(string name)
    // {
    //     UiAnimatedElement anim;
    //     anim = Animations_GetByName(name);
    //     if (anim == null)
    //     {
    //         Debug.LogError("Nome dell'animazione non trovata!");
    //         return false;
    //     }
    //     else return anim.IsOnEmptyState();
    // }

    // public bool Animations_IsAnyNotInEmptyState()
    // {
    //     var firstMatch = Array.Find(_standardAnimations, elem => elem.IsOnEmptyState() == false);
    //     return firstMatch == null ? false : true;
    // }

    // public bool Animations_IsAnyNotInEmptyState(UiAnimatedElement[] animations)
    // {
    //     var firstMatch = Array.Find(animations, elem => elem.IsOnEmptyState() == false);
    //     return firstMatch == null ? false : true;
    // }

    public bool Animations_IsAnyPlaying(string stateName)
    {
        var firstMatch = Array.Find(_standardAnimations, elem => elem.IsPlaying(stateName) == true);
        return firstMatch == null ? true : false;
    }

    public bool Animations_IsAnyPlaying(UiAnimatedElement[] animations, string stateName)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsPlaying(stateName) == true);
        return firstMatch == null ? true : false;
    }


    //#endregion







    //#region LOTTIE ANIMATIONS MANAGER

    public void Lottie_PlayByName(string assetName)
    {
        foreach (var anim in _lottieAnimations)
        {
            if (anim.Name == assetName)
            {
                /// For additive shader
                anim.material.SetFloat("_Opacity", 1f);

                /// For legacy UI unlit transparent shader
                Color c = new Color(1f, 1f, 1f, 1f);
                anim.material.SetColor("_Color", c);

                anim.Play();
            }
        }
    }
    public void Lottie_StopByName(string assetName)
    {
        foreach (var anim in _lottieAnimations)
        {
            if (anim.Name == assetName) anim.Stop();
        }
    }
    public void Lottie_PlayAll()
    {
        foreach (var anim in _lottieAnimations) anim.Play();
    }
    public void Lottie_StopAll()
    {
        foreach (var anim in _lottieAnimations) anim.Stop();
    }
    public float Lottie_GetDuration_ByName(string assetName)
    {
        var anim = Array.Find(_lottieAnimations, elem => elem.Name == assetName);
        return anim.GetDuration();
    }


    //#endregion
}
