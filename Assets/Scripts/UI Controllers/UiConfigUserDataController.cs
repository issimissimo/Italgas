using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;
using System.Threading.Tasks;


public class UiConfigUserDataController : MonoBehaviour
{
    [SerializeField] CustomDropdown _dropDown;
    [SerializeField] CanvasController _playerModeCanvas;
    [SerializeField] SwitchManager _soloModeSwitcher;
    [SerializeField] TMP_InputField _dataUrlField;
    [SerializeField] TMP_InputField _ftpUserNameField;
    [SerializeField] TMP_InputField _ftpPasswordField;
    [SerializeField] TMP_InputField _ftpServerField;
    [SerializeField] TMP_InputField _ftpFolderField;
    [SerializeField] List<Toggle> _playerIdToggles;
    [SerializeField] GameObject _saveUserDataButton;
    [SerializeField] GameObject _continueButton;

    private bool _isUpdatingUi = false;



    private void Awake()
    {
        _dropDown.dropdownEvent.AddListener(OnDropdownChanged);
    }

    private void OnDisable()
    {
        _dropDown.dropdownEvent.RemoveAllListeners();
    }

    public async void UpdateUI()
    {
        _isUpdatingUi = true;

        if (GameManager.userData.gameMode == Globals.GAMEMODE.PLAYER) _dropDown.ChangeDropdownInfo(0);
        if (GameManager.userData.gameMode == Globals.GAMEMODE.VIEWER) _dropDown.ChangeDropdownInfo(1);

        _continueButton.SetActive(GameManager.instance.isDevelopment ? true : false);

        _playerIdToggles[GameManager.userData.playerId].isOn = true;

        _dataUrlField.text = GameManager.userData.dataUrl;
        _ftpUserNameField.text = GameManager.userData.ftpUserName;
        _ftpPasswordField.text = GameManager.userData.ftpPassword;
        _ftpServerField.text = GameManager.userData.ftpServer;
        _ftpFolderField.text = GameManager.userData.ftpFolder;

        /// delay because the switcher does not work without it...
        await Task.Delay(500);
        if (GameManager.userData.requestedPlayers == 1) _soloModeSwitcher.SetOn();
        if (GameManager.userData.requestedPlayers == 2) _soloModeSwitcher.SetOff();

        _isUpdatingUi = false;
    }

    public async void OnSomeValueChanged()
    {
        if (_isUpdatingUi) return;

        /// We must wait for stuff that update in the UI...
        await Task.Yield();

        GameManager.userData.Set(
            gameMode: _dropDown.selectedItemIndex == 0 ? Globals.GAMEMODE.PLAYER : Globals.GAMEMODE.VIEWER,
            playerId: _playerIdToggles.FindIndex(a => a.isOn == true),
            requestedPlayers: _soloModeSwitcher.isOn ? 1 : 2,
            dataUrl: _dataUrlField.text,
            ftpUserName: _ftpUserNameField.text,
            ftpPassword: _ftpPasswordField.text,
            ftpServer: _ftpServerField.text,
            ftpFolder: _ftpFolderField.text
        );
    }


    private void OnDropdownChanged(int value)
    {
        if (value == 0) _playerModeCanvas.Toggle(true);
        if (value == 1) _playerModeCanvas.Toggle(false);
    }


}
