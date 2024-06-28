using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircleFillHandler : MonoBehaviour
{
    [Range(0, 100)]
    public float fillValue = 0;
    [SerializeField] float fillTime = 2f;
    [SerializeField] Image circleFillImage;
    [SerializeField] RectTransform handlerEdgeImage;
    [SerializeField] RectTransform fillHandler;
    [SerializeField] TMP_Text textValue;
    [SerializeField] TMP_Text textMaxValue;
    public AnimationsController anim;


    public float MaxValue
    {
        set
        {
            _maxValue = value;
            if (textMaxValue != null) textMaxValue.text = ((int)_maxValue).ToString();
        }
    }
    private float _maxValue;

    // private const float _minFillValue = 0f;
    // private const float _maxFillValue = 95f;



    // void Start()
    // {
    //     MaxValue = 10f;
    //     StartCoroutine(SetValue(3f));
    // }



    public IEnumerator SetValue(float value)
    {
        float percent = Mathf.InverseLerp(0f, _maxValue, value) * 100f;
        print("percent: " + percent);

        float time = fillTime * percent / 100f;
        print("time: " + time);

        float t = 0f;

        while (t <= time)
        {
            t += Time.deltaTime;

            float lerpValue = t / time;
            lerpValue = lerpValue*lerpValue*lerpValue * (lerpValue * (6f*lerpValue - 15f) + 10f);

            fillValue = Mathf.Lerp(0f, percent, lerpValue);
            textValue.text = ((int)Mathf.Lerp(0f, value, lerpValue)).ToString();

            FillCircleValue(fillValue);

            yield return null;

        }



    }


    // void Update()
    // {
    //     FillCircleValue(fillValue);
    // }

    void FillCircleValue(float value)
    {
        float fillAmount = (value / 100.0f);
        circleFillImage.fillAmount = fillAmount;
        float angle = fillAmount * 360;
        fillHandler.localEulerAngles = new Vector3(0, 0, -angle);
        handlerEdgeImage.localEulerAngles = new Vector3(0, 0, angle);
    }
}
