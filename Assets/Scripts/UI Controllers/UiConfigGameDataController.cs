using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using System;
using UnityEngine.UI;

public class UiConfigGameDataController : MonoBehaviour
{
    /// SubControllers
    [SerializeField] UiConfigGameDataVersionSubController _versionController;
    [SerializeField] UiConfigGameDataChapterSubController _chapterController;
    [SerializeField] UiConfigGameDataPageSubController _pageController;


    [SerializeField] CanvasController _canvasContent;
    [SerializeField] CustomDropdown _gameVersionDropDown;
    [SerializeField] List<Toggle> _selectors;
    [SerializeField] List<GameObject> _contents;
    [SerializeField] HorizontalSelector _versionSelector;
    [SerializeField] HorizontalSelector _chapterSelector;
    [SerializeField] HorizontalSelector _pageSelector;
    [SerializeField] GameObject _removePageButton;
    [SerializeField] Sprite _usersSprite;

    public Data.VERSION_NAME oldVersionName { get; private set; }

    private Data.VERSION_NAME _selectedVersionName;
    private int _selectedChapterIndex = 0;
    private int _selectedPageIndex = 0;

    private bool _isUpdatingUi;



    private void Awake()
    {
        _gameVersionDropDown.dropdownEvent.AddListener(OnDropdownGameVersionChange);
    }

    private void OnDisable()
    {
        _gameVersionDropDown.dropdownEvent.RemoveAllListeners();
    }

    public void UpdateUI()
    {
        _isUpdatingUi = true;

        Data.imagesToUploadLocalPathList = new List<string>();
        Data.imagesToUploadNameList = new List<string>();

        if (!GameManager.userData.configurationComplete)
        {
            _canvasContent.Toggle(false);
            return;
        }

        _canvasContent.Toggle(true);

        oldVersionName = GameManager.gameData.currentVersion;

        /// create dropdown
        if (_gameVersionDropDown.dropdownItems.Count == 0)
        {
            foreach (string name in Enum.GetNames(typeof(Data.VERSION_NAME)))
                _gameVersionDropDown.CreateNewItem(name, _usersSprite);
        }

        /// create version selector
        if (_versionSelector.itemList.Count == 0)
        {
            foreach (string name in Enum.GetNames(typeof(Data.VERSION_NAME)))
                _versionSelector.CreateNewItem(name);

            _versionSelector.defaultIndex = (int)GameManager.gameData.currentVersion;
            _versionSelector.SetupSelector();
        }
        _gameVersionDropDown.ChangeDropdownInfo((int)GameManager.gameData.currentVersion);
        OnSelectorVersionChange((int)GameManager.gameData.currentVersion);

        /// we don't need to create the chapter selector (always 3 items)
        _chapterSelector.defaultIndex = 0;
        _selectedChapterIndex = 0;

        OnToggleSelectorChange();

        _isUpdatingUi = false;
    }

    /// Clicked Toggle Selectors
    public void OnToggleSelectorChange()
    {
        GameManager.instance.PlayAudioSuperSoftClick();
        
        int selected = _selectors.FindIndex(a => a.isOn == true);
        print("OnToggleSelectorChange: " + selected);
        for (int i = 0; i < _contents.Count; i++)
        {
            _contents[i].SetActive(i == selected ? true : false);
        }

        /// SHITTTTTT!!!!!!! MODO SPORCHISSIMO PER RISOLVERE IL PROBLEMA
        /// DEI TOGGLES ALL'APERTURA DELLA PAGINA
        if (selected == 2)
        {
            _pageController.UpdateUI(_selectedVersionName, _selectedChapterIndex, _selectedPageIndex);
            _pageController.SetTogglesEditable();
        }
    }


    /// Changed Version Selector
    public void OnSelectorVersionChange(int index)
    {
        GameManager.instance.PlayAudioSuperSoftClick();

        /// Update Version UI
        _selectedVersionName = (Data.VERSION_NAME)index;
        _versionController.UpdateUI(_selectedVersionName);

        OnSelectorChapterChange(0);

        /// Reset Chapter selector  
        _chapterSelector.index = 0;
        _chapterSelector.UpdateUI();
    }

    /// Changed Chapter Selector
    public void OnSelectorChapterChange(int index)
    {
        GameManager.instance.PlayAudioSuperSoftClick();
        
        /// Update Chapter UI
        _selectedChapterIndex = index;
        _chapterController.UpdateUI(_selectedVersionName, _selectedChapterIndex);

        /// Update Page Selector
        Data.GameVersion selectedVersion = GameManager.gameData.GetVersion(_selectedVersionName);
        int chapterPageNumber = selectedVersion.chapters[_selectedChapterIndex].pages.Count;
        _pageSelector.SetupItemsOnNumbers(chapterPageNumber);

        OnSelectorPageChange(0);
    }

    /// Changed Page Selector
    public void OnSelectorPageChange(int index)
    {
        GameManager.instance.PlayAudioSuperSoftClick();

        /// Update Page UI
        _selectedPageIndex = index;
        _pageController.UpdateUI(_selectedVersionName, _selectedChapterIndex, _selectedPageIndex);

        /// Set RemovePage button
        Data.GameChapter chapter = GameManager.gameData.GetVersion(_selectedVersionName).chapters[_selectedChapterIndex];
        _removePageButton.SetActive(chapter.pages.Count > 1 ? true : false);
    }

    /// Add page to current chapter
    public void AddPage()
    {
        GameManager.instance.PlayAudioClick();
        
        Data.GameChapter chapter = GameManager.gameData.GetVersion(_selectedVersionName).chapters[_selectedChapterIndex];
        Data.GamePage page = new Data.GamePage
        {
            answers = new List<Data.GameAnswer>()
        };
        chapter.pages.Add(page);

        /// Update Page Selector
        _pageSelector.SetupItemsOnNumbers(chapter.pages.Count);
        _pageSelector.index = chapter.pages.Count - 1;
        _pageSelector.UpdateUI();

        /// Refresh page
        OnSelectorPageChange(chapter.pages.Count - 1);
    }

    /// Remove page from current chapter
    public void RemovePage()
    {
        Data.GameChapter chapter = GameManager.gameData.GetVersion(_selectedVersionName).chapters[_selectedChapterIndex];
    }


    private void OnDropdownGameVersionChange(int value)
    {
        GameManager.gameData.currentVersion = (Data.VERSION_NAME)value;
        // print("game version is: " + GameManager.gameData.currentVersion.ToString());
    }


}
