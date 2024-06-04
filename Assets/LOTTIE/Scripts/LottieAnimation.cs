using UnityEngine;
using UnityEngine.UI;
using LottiePlugin.UI;

[RequireComponent(typeof(AnimatedImage))]
public class LottieAnimation : MonoBehaviour
{
    public string Name { get; private set; }
    public RawImage rawImage { get; private set; }
    public Material material { get; private set; }
    private AnimatedImage _lottieAsset;

    void Awake()
    {
        _lottieAsset = GetComponent<AnimatedImage>();
        Name = _lottieAsset.GetAssetName(); /// Not true! It work
        rawImage = _lottieAsset.GetRawImage();
        material = rawImage.material;
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
