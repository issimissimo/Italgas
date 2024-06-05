using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class GamePanelSubControllerBase : MonoBehaviour
{
    public UiController.STATE STATE;



    protected float timer { get; private set; }

    [Header("LOTTIE ANIMATIONS")]
    [SerializeField] protected LottieAnimation[] _lottieAnimations;

    [Header("STANDARD ANIMATIONS")]
    [SerializeField] protected UiAnimatedElement[] _standardAnimations;

    // public virtual void SetUI_on_STATE()
    // {
    //     /// To override
    // }
    // public virtual void SetUI_on_RUNNING_STATE(UiController.RUNNING_STATE runningState, Action callback = null)
    // {
    //     /// To override
    // }





    public virtual void SetupUI(UiController.STATE state, UiController.RUNNING_STATE runningState, Action callback)
    {
        /// To override
    }





    /// <summary>
    /// CLOSE ALL ANIMATED ELEMENTS OF THIS UI CONTROLLER
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator CloseAllAnimatedElements()
    {
        Animations_ExitAll();
        Lottie_FadeOut_All(1f);

        yield return null;

        while (Animations_IsAnyNotInEmptyState() || _lottie_isFading)
            yield return null;
    }






    //#region STANDARD ANIMATIONS MANAGER

    protected void Animations_EnterAll()
    {
        foreach (var a in _standardAnimations) a.Enter();
    }

    protected void Animations_ExitAll()
    {
        foreach (var a in _standardAnimations) a.Exit();
    }

    protected void Animations_EnterByName(string name)
    {
        foreach (var anim in _standardAnimations) if (anim.Name == name) anim.Enter();
    }

    protected void Animations_ExitByName(string name)
    {
        foreach (var anim in _standardAnimations) if (anim.Name == name) anim.Exit();
    }

    protected UiAnimatedElement Animations_GetByName(string name)
    {
        foreach (var anim in _standardAnimations)
        {
            if (anim.Name == name) return anim;
        }
        return null;
    }

    protected bool Animations_IsInEmptyState(string name)
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

    protected bool Animations_IsAnyNotInEmptyState()
    {
        var firstMatch = Array.Find(_standardAnimations, elem => elem.IsOnEmptyState() == false);
        return firstMatch == null ? false : true;
    }

    protected bool Animations_IsAnyNotInEmptyState(UiAnimatedElement[] animations)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsOnEmptyState() == false);
        return firstMatch == null ? false : true;
    }

    protected bool Animations_IsAnyPlaying(string stateName)
    {
        var firstMatch = Array.Find(_standardAnimations, elem => elem.IsPlaying(stateName) == true);
        return firstMatch == null ? true : false;
    }

    protected bool Animations_IsAnyPlaying(UiAnimatedElement[] animations, string stateName)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsPlaying(stateName) == true);
        return firstMatch == null ? true : false;
    }


    //#endregion







    //#region LOTTIE ANIMATIONS MANAGER

    protected bool _lottie_isFading;

    private Coroutine _lottie_setMaterialOpacity;

    protected void Lottie_PlayByName(string assetName)
    {
        foreach (var l in _lottieAnimations)
        {
            if (l.Name == assetName) l.Play();
        }
    }
    protected void Lottie_StopByName(string assetName)
    {
        foreach (var l in _lottieAnimations)
        {
            if (l.Name == assetName) l.Stop();
        }
    }
    protected IEnumerator Lottie_Stop_All_Coroutine()
    {
        foreach (var anim in _lottieAnimations) anim.Stop();
        yield return null;
    }


    protected void Lottie_FadeIn_All(float? time = null)
    {
        _lottie_isFading = true;
        float fadeTime = time != null ? time.Value : 0.5f;

        if (_lottie_setMaterialOpacity != null)
        {
            StopCoroutine(_lottie_setMaterialOpacity);
            _lottie_setMaterialOpacity = null;
        }

        foreach (var anim in _lottieAnimations)
        {
            anim.isFadedIn = true;
            anim.Play();
        }

        _lottie_setMaterialOpacity = StartCoroutine(Lottie_SetMaterialsOpacityCoroutine(fadeTime, 0f, 1f, () =>
        {
            _lottie_isFading = false;
        }));
    }


    protected void Lottie_FadeOut_All(float? time = null)
    {
        _lottie_isFading = true;
        float fadeTime = time != null ? time.Value : 0.5f;

        if (_lottie_setMaterialOpacity != null)
        {
            StopCoroutine(_lottie_setMaterialOpacity);
            _lottie_setMaterialOpacity = null;
        }

        _lottie_setMaterialOpacity = StartCoroutine(Lottie_SetMaterialsOpacityCoroutine(fadeTime, 1f, 0f, () =>
        {
            foreach (var anim in _lottieAnimations)
            {
                anim.isFadedIn = false;
                anim.Stop();
            }

            _lottie_isFading = false;
        }));
    }



    private IEnumerator Lottie_SetMaterialsOpacityCoroutine(float time, float initOpacity, float endOpacity, Action callback)
    {
        List<Material> materials = new List<Material>();
        Material oldMat = null;
        foreach (var anim in _lottieAnimations)
        {
            if (anim.material != oldMat)
            {
                materials.Add(anim.material);
                oldMat = anim.material;
            }
        }

        float t = 0;
        while (t <= time)
        {
            t += Time.deltaTime;
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].SetFloat("_Opacity", Mathf.Lerp(initOpacity, endOpacity, t / time));
            }
            yield return null;
        }

        _lottie_setMaterialOpacity = null;

        if (callback != null) callback.Invoke();
    }

    //#endregion


    /// <summary>
    /// ////////////// TIMER
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="callback"></param>
    /// 

    private Coroutine _timerCoroutine;


    protected void StartTimer(float seconds, Action callback = null)
    {
        if (_timerCoroutine != null) StopTimer();
        _timerCoroutine = StartCoroutine(TimerCoroutine(seconds, callback));
    }

    protected void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    private IEnumerator TimerCoroutine(float seconds, Action callback)
    {
        timer = 0f;
        while (timer < seconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (callback != null) callback.Invoke();
    }
}
