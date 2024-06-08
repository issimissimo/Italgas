using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasController : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Toggle(bool value)
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = value ? 1f : 0.2f;
        _canvasGroup.interactable = value ? true : false;
    }

    public void SetOn()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
    public void SetOff()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

}
