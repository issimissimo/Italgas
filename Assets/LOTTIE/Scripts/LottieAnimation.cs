using UnityEngine;
using LottiePlugin.UI;

[RequireComponent(typeof(AnimatedImage))]
public class LottieAnimation : MonoBehaviour
{
    public string Name { get; private set; }
    private AnimatedImage _lottie;

    void Awake()
    {
        _lottie = GetComponent<AnimatedImage>();
        Name = _lottie.GetAssetName(); /// Not true! It work

    }

    public void Play()
    {
        _lottie.Play();
    }
    public void Stop()
    {
        _lottie.Stop();
    }
}
