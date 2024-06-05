using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Lottie : MonoBehaviour
{
    public static Lottie instance;

    public bool isFading { get; private set; }

    private Coroutine SetMaterialOpacity;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void PlayByName(string assetName, LottieAnimation[] lottieAnimations)
    {
        foreach (var l in lottieAnimations)
        {
            if (l.Name == assetName) l.Play();
        }
    }
    public void StopByName(string assetName, LottieAnimation[] lottieAnimations)
    {
        foreach (var l in lottieAnimations)
        {
            if (l.Name == assetName) l.Stop();
        }
    }
    public IEnumerator Stop_All_Coroutine()
    {
        LottieAnimation[] _lottieAnimations = FindObjectsOfType<LottieAnimation>();
        foreach (var anim in _lottieAnimations) anim.Stop();
        yield return null;
    }


    public void FadeIn(LottieAnimation[] lottieAnimations, float? time = null)
    {
        isFading = true;
        float fadeTime = time != null ? time.Value : 0.5f;

        if (SetMaterialOpacity != null)
        {
            StopCoroutine(SetMaterialOpacity);
            SetMaterialOpacity = null;
        }

        foreach (var anim in lottieAnimations) anim.Play();

        SetMaterialOpacity = StartCoroutine(SetMaterialsOpacityCoroutine(lottieAnimations, fadeTime, 0f, 1f, () =>
        {
            isFading = false;
        }));
    }


    public void FadeOut(LottieAnimation[] lottieAnimations, float? time = null)
    {
        isFading = true;
        float fadeTime = time != null ? time.Value : 0.5f;

        if (SetMaterialOpacity != null)
        {
            StopCoroutine(SetMaterialOpacity);
            SetMaterialOpacity = null;
        }

        SetMaterialOpacity = StartCoroutine(SetMaterialsOpacityCoroutine(lottieAnimations, fadeTime, 1f, 0f, () =>
        {
            foreach (var anim in lottieAnimations) anim.Stop();
            isFading = false;
        }));
    }



    private IEnumerator SetMaterialsOpacityCoroutine(LottieAnimation[] lottieAnimations, float time, float initOpacity, float endOpacity, Action callback)
    {
        List<Material> materials = new List<Material>();
        Material oldMat = null;
        foreach (var anim in lottieAnimations)
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

        SetMaterialOpacity = null;

        if (callback != null) callback.Invoke();
    }
}
