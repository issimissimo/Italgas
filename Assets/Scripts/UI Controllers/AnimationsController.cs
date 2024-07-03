using System.Collections;
using UnityEngine;
using System;
using UnityExtensions.Tween;

public class AnimationsController : MonoBehaviour
{
    [SerializeField] protected TweenPlayer[] _tweenAnimations;
    [SerializeField] protected AudioSource[] _audioSources;
    [SerializeField] protected LottieAnimation[] _lottieAnimations;

   


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


    public IEnumerator Tween_PlayByNameWithDelay(string name, float delay, Action OnEnd = null)
    {
        yield return new WaitForSeconds(delay);
        Tween_PlayByName(name, OnEnd);
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




    //#region AUDIO SOURCES MANAGER

    public void Audio_PlayByName(string name)
    {
        foreach (var audio in _audioSources)
        {
            if (audio.gameObject.name == name)
            {
                audio.Play();
                return;
            }
        }
        Debug.LogError("Tween Animation  '" + name + "' can't be found");
    }

    public IEnumerator Audio_PlayByNameWithDelay(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        Audio_PlayByName(name);
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
