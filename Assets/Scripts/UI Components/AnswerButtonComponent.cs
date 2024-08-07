using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButtonComponent : MonoBehaviour
{
    public AnimationsController animationsController;
    public TMP_Text answerText;
    public Button button;
    public bool isTrue { get; private set; }
    public int buttonNumber { get; private set; }

    void OnEnable()
    {
        /// We need this to hide the button when the page is just opened (!!!?)
        GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void Setup(string text, bool result, int number)
    {
        answerText.text = text;
        isTrue = result;
        buttonNumber = number;
    }
}
