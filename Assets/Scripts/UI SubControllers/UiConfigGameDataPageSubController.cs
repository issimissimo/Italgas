using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;
using System.Threading.Tasks;

public class UiConfigGameDataPageSubController : MonoBehaviour
{
    [SerializeField] TMP_InputField _questionField;
    [SerializeField] List<TMP_InputField> _answerFields;
    [SerializeField] List<Toggle> _answerToggles;

    private Data.GamePage _gamePage;
    private bool _isUpdatingUI;
    private CustomInputField _customInputField;

    
    /// FUCKING WAY TO SOLVE THE TOGGLES PROBLEM !!!
    private bool _togglesEditable = false;
    public void SetTogglesEditable()
    {
        _togglesEditable = true;
    }


    public async void UpdateUI(Data.VERSION_NAME versionName, int chapterIndex, int pageIndex)
    {
        _isUpdatingUI = true;

        Data.GameVersion version = GameManager.gameData.GetVersion(versionName);
        Data.GameChapter chapter = version.chapters[chapterIndex];
        _gamePage = chapter.pages[pageIndex];

        _questionField.text = _gamePage.question;
        _customInputField = _questionField.GetComponent<CustomInputField>();
        _customInputField.UpdateState();

        for (int i = 0; i < _answerFields.Count; i++)
        {
            if (_gamePage.answers.Count > i)
            {
                _answerFields[i].text = _gamePage.answers[i].title;

                if (_gamePage.answers[i].isTrue)
                {
                    _answerToggles[i].isOn = true;
                }
            }
            else
            {
                _answerFields[i].text = "";
            }

            _customInputField = _answerFields[i].GetComponent<CustomInputField>();
            _customInputField.UpdateState();
        }

        await Task.Yield();
        _isUpdatingUI = false;
    }

    public void OnInputFieldChanged()
    {
        SetData();
    }


    public void OnToggleChanged(bool value)
    {
        if (!_togglesEditable) return;
        SetData();
    }

    private void SetData()
    {
        if (_isUpdatingUI) return;

        _gamePage.question = _questionField.text;
        _gamePage.answers.Clear();

        int rightAnswer = _answerToggles.FindIndex(a => a.isOn == true);

        for (int i = 0; i < _answerFields.Count; i++)
        {
            Data.GameAnswer answer = new Data.GameAnswer
            {
                title = _answerFields[i].text,
                isTrue = i == rightAnswer ? true : false
            };

            _gamePage.answers.Add(answer);
        }
    }
}
