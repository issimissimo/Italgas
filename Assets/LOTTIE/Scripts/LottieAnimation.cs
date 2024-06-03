using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LottiePlugin.UI;

[RequireComponent(typeof(AnimatedImage))]
public class LottieAnimation : MonoBehaviour
{
    AnimatedImage _lottie;
    
    void Awake()
    {
        _lottie = GetComponent<AnimatedImage>();
    }
}
