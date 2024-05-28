using UnityEngine;
using Michsky.UI.ModernUIPack;

public class UiConfigGameDataVersionSubController : MonoBehaviour
{
    [SerializeField] RadialSlider _maxTimeSlider;
    private Data.GameVersion _version;


    public void UpdateUI(Data.VERSION_NAME versionName)
    {
        _version = GameManager.gameData.GetVersion(versionName);

        _maxTimeSlider.currentValue = _version.maxTimeInSeconds / 60f;
        _maxTimeSlider.ForceUpdate();
    }


    public void OnSliderValueChanged(float value)
    {
        if (_version != null)
        {
            _version.maxTimeInSeconds = _maxTimeSlider.currentValue * 60f;
        }
    }
}
