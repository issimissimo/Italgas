using UnityEngine;
using UnityEngine.UI;
using LottiePlugin.UI;

[RequireComponent(typeof(AnimatedImage))]
public class LottieAnimation : MonoBehaviour
{
    [SerializeField] private float _duration;
    public string Name { get; private set; }
    public bool isFadedIn { get; set; }
    public RawImage rawImage { get; private set; }
    public Material material { get; private set; }
    public float opacity;
    private float _opacity;
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

    public float GetDuration()
    {
        if (_duration == 0f) Debug.LogError("You are trying to get a Lottie duration, but it's not specified!");
        return _duration;
    }
}
