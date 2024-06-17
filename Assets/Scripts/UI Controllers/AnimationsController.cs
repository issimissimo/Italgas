using System.Collections;
using UnityEngine;
using System;
using UnityExtensions.Tween;

public class AnimationsController : MonoBehaviour
{
    [SerializeField] protected TweenPlayer[] _tweenAnimations;

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



    //#region TWEEN ANIMATIONS MANAGER

    void OnEnable()
    {
        foreach (var a in _tweenAnimations) a.enabled = false;
    }


    public void Tween_PlayByName(string name, Action OnEnd = null)
    {
        foreach (var anim in _tweenAnimations)
        {
            if (anim.gameObject.name == name)
            {
                Tween_PlayForward(anim, OnEnd);
                return;
            }
        }
        Debug.LogError("Tween Animation  '" + name + "' can't be found");
    }


    public IEnumerator Tween_PlayWithDelay(TweenPlayer[] animations, float delay, Action callback)
    {
        float delayTime = delay;
        for (int i = 0; i < animations.Length; i++)
        {
            yield return new WaitForSeconds(delayTime);
            animations[i].SetForwardDirectionAndEnabled();
            delayTime += delay;
        }
        callback?.Invoke();
    }



    /// <summary>
    /// Play Forward the Tween (NOTE: use "OnEnd" just once!)
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="OnEnd"></param>
    private void Tween_PlayForward(TweenPlayer anim, Action OnEnd)
    {
        if (OnEnd != null) anim.onForwardArrived += () => OnEnd();
        anim.normalizedTime = 0f;
        anim.SetForwardDirectionAndEnabled();
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

        // /// Trick, because immediately don't work
        // while (!Animations_IsAnyPlaying("Exit"))
        // {
        // yield return null;
        // }
        yield return new WaitForSeconds(0.1f);

        while (Animations_IsAnyPlaying("Exit"))
        {
            yield return null;
        }

    }

    public void Animations_EnterByName(string animatorName)
    {
        // foreach (var anim in _standardAnimations) if (anim.animatorName == animatorName) anim.Enter();
        var anim = Animations_GetByName(animatorName);
        if (anim != null) anim.Enter();
    }

    public void Animations_ExitByName(string animatorName)
    {
        // foreach (var anim in _standardAnimations) if (anim.animatorName == animatorName) anim.Exit();
        var anim = Animations_GetByName(animatorName);
        if (anim != null) anim.Exit();
    }

    public void Animations_PlayByName(string animatorName, string animationName)
    {
        var anim = Animations_GetByName(animatorName);
        if (anim != null) anim.PlayAnimation(animationName);
    }

    public UiAnimatedElement Animations_GetByName(string animatorName)
    {
        foreach (var anim in _standardAnimations)
            if (anim.animatorName == animatorName) return anim;

        Debug.LogError("The Animator '" + animatorName + "' on gameObject " + gameObject.name + " can't be found!");
        return null;
    }

    public bool Animations_IsAnyPlaying(string animationName)
    {
        var firstMatch = Array.Find(_standardAnimations, elem => elem.IsPlaying(animationName) == true);
        return firstMatch == null ? false : true;
    }

    public bool Animations_IsAnyPlaying(UiAnimatedElement[] animations, string animationName)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsPlaying(animationName) == true);
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
