using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Lottie : MonoBehaviour
{
    public static Lottie instance;
    
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
    public IEnumerator StopAll_Coroutine()
    {
        LottieAnimation[] _lottieAnimations = FindObjectsOfType<LottieAnimation>();
        foreach (var anim in _lottieAnimations) anim.Stop();
        yield return null;
    }
}
