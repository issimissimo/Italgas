using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FitImageToScreen : MonoBehaviour
{
    private enum ScreenPosition { left = 1, right = -1 }
    [SerializeField] private ScreenPosition _screenPosition;

    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 rectPos = new Vector2(Screen.width / 4 * (int)_screenPosition, 0);
        rect.anchoredPosition = rectPos;
        Vector2 rectSize = new Vector2(Screen.width / 2, 0);
        rect.sizeDelta = rectSize;
    }


}
