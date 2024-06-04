using UnityEngine;
using LottiePlugin.UI;

[RequireComponent(typeof(AnimatedImage))]
public class LottieAnimation : MonoBehaviour
{
    public string Name { get; private set; }
    private AnimatedImage _lottieAsset;

    void Awake()
    {
        _lottieAsset = GetComponent<AnimatedImage>();
        Name = _lottieAsset.GetAssetName(); /// Not true! It work

    }

    public void Play()
    {
        _lottieAsset.Play();
    }
    public void Stop()
    {
        _lottieAsset.Stop();
    }
}
