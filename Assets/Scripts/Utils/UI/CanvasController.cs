using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;


    public void Toggle(bool value)
    {
        _canvasGroup.alpha = value ? 1f : 0.2f;
        _canvasGroup.interactable = value ? true : false;
    }

    public void SetOn()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
    public void SetOff()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

}
