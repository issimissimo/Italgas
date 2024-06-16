using UnityEngine;
using UnityEngine.UI;
using LottiePlugin.UI;
using UnityEngine.Serialization;

[RequireComponent(typeof(AnimatedImage))]
public class LottieAnimation : MonoBehaviour
{
    [SerializeField] private float _duration;
    public string Name { get; private set; }
    // public bool isFadedIn { get; set; }
    public RawImage rawImage { get; private set; }
    public Material material { get; private set; }



 



    // public int thisIsMyOldField
    // {
    //     get => _thisIsMyOldField;//whatever
    //     set => _thisIsMyOldField = value;//whatever
    // }

    // [FormerlySerializedAs("thisIsMyOldField")]
    // [SerializeField] private int _thisIsMyOldField;

    // public float opacity
    // {
    //     get { return _opacity; }
    //     set
    //     {
    //         print("AAAAAAAAAAAAA");


    //         _opacity = value;
    //         material.SetFloat("_Opacity", _opacity);

    //     }
    // }


    // [FormerlySerializedAs("opacity")]
    // [SerializeField] private float _opacity = 1f;




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
