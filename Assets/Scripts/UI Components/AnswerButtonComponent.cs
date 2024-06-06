using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButtonComponent : MonoBehaviour
{
    public AnimationsController animationsController;
    public TMP_Text answerText;
    public UiAnimatedElement animatedElement;
    public Button button;
    public bool isTrue { get; private set; }
    public int buttonNumber { get; private set; }

    public void Set(string text, bool result, int number)
    {
        answerText.text = text;
        isTrue = result;
        buttonNumber = number;
    }
}
