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



    //#region STANDARD ANIMATIONS MANAGER

    public void Animations_EnterAll()
    {
        foreach (var a in _standardAnimations) a.Enter();
    }

    public IEnumerator Animations_ExitAll()
    {
        foreach (var a in _standardAnimations) a.Exit();

        yield return null;

        while (Animations_IsAnyNotInEmptyState())
            yield return null;
    }

    public void Animations_EnterByName(string name)
    {
        foreach (var anim in _standardAnimations) if (anim.Name == name) anim.Enter();
    }

    public void Animations_ExitByName(string name)
    {
        foreach (var anim in _standardAnimations) if (anim.Name == name) anim.Exit();
    }

    public UiAnimatedElement Animations_GetByName(string name)
    {
        foreach (var anim in _standardAnimations)
        {
            if (anim.Name == name) return anim;
        }
        return null;
    }

    public bool Animations_IsInEmptyState(string name)
    {
        UiAnimatedElement anim;
        anim = Animations_GetByName(name);
        if (anim == null)
        {
            Debug.LogError("Nome dell'animazione non trovata!");
            return false;
        }
        else return anim.IsOnEmptyState();
    }

    public bool Animations_IsAnyNotInEmptyState()
    {
        var firstMatch = Array.Find(_standardAnimations, elem => elem.IsOnEmptyState() == false);
        return firstMatch == null ? false : true;
    }

    public bool Animations_IsAnyNotInEmptyState(UiAnimatedElement[] animations)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsOnEmptyState() == false);
        return firstMatch == null ? false : true;
    }

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

    public bool _lottie_isFading { get; private set; }

    // private Coroutine _lottie_setMaterialOpacity;

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
    // public void Lottie_FadeIn_All(float? time = null)
    // {
    //     _lottie_isFading = true;
    //     float fadeTime = time != null ? time.Value : 0.5f;

    //     if (_lottie_setMaterialOpacity != null)
    //     {
    //         StopCoroutine(_lottie_setMaterialOpacity);
    //         _lottie_setMaterialOpacity = null;
    //     }

    //     foreach (var anim in _lottieAnimations)
    //     {
    //         anim.isFadedIn = true;
    //         anim.Play();
    //     }

    //     _lottie_setMaterialOpacity = StartCoroutine(Lottie_SetMaterialsOpacityCoroutine(fadeTime, 0f, 1f, () =>
    //     {
    //         _lottie_isFading = false;
    //     }));
    // }


    // public void Lottie_FadeOut_All(float? time = null)
    // {
    //     _lottie_isFading = true;
    //     float fadeTime = time != null ? time.Value : 0.5f;

    //     if (_lottie_setMaterialOpacity != null)
    //     {
    //         StopCoroutine(_lottie_setMaterialOpacity);
    //         _lottie_setMaterialOpacity = null;
    //     }

    //     _lottie_setMaterialOpacity = StartCoroutine(Lottie_SetMaterialsOpacityCoroutine(fadeTime, 1f, 0f, () =>
    //     {
    //         foreach (var anim in _lottieAnimations)
    //         {
    //             anim.isFadedIn = false;
    //             anim.Stop();
    //         }

    //         _lottie_isFading = false;
    //     }));
    // }

    public float Lottie_GetDuration_ByName(string assetName)
    {
        var anim = Array.Find(_lottieAnimations, elem => elem.Name == assetName);
        return anim.GetDuration();
    }



    // private IEnumerator Lottie_SetMaterialsOpacityCoroutine(float time, float initOpacity, float endOpacity, Action callback)
    // {
    //     List<Material> materials = new List<Material>();
    //     Material oldMat = null;
    //     foreach (var anim in _lottieAnimations)
    //     {
    //         if (anim.material != oldMat)
    //         {
    //             materials.Add(anim.material);
    //             oldMat = anim.material;
    //         }
    //     }

    //     float t = 0;
    //     while (t <= time)
    //     {
    //         t += Time.deltaTime;
    //         for (int i = 0; i < materials.Count; i++)
    //         {
    //             float opacity = Mathf.Lerp(initOpacity, endOpacity, t / time);

    //             /// For additive shader
    //             materials[i].SetFloat("_Opacity", opacity);

    //             /// For legacy UI unlit transparent shader
    //             Color c = new Color(1f, 1f, 1f, opacity);
    //             materials[i].SetColor("_Color", c);
    //         }
    //         yield return null;
    //     }

    //     _lottie_setMaterialOpacity = null;

    //     if (callback != null) callback.Invoke();
    // }

    //#endregion
}
