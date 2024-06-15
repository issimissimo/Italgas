using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Threading.Tasks;
using SFB;
using Michsky.UI.ModernUIPack;

public class UiConfigGameDataChapterSubController : MonoBehaviour
{
    [SerializeField] TMP_InputField _chapterNameInputField;
    [SerializeField] TMP_Text _backgroundImageNameText;
    [SerializeField] RawImage _backgroundImage;

    private Data.GameChapter _chapter;
    private bool _isUpdatingUI;


    public async void UpdateUI(Data.VERSION_NAME versionName, int chapterIndex)
    {
        _isUpdatingUI = true;

        Data.GameVersion version = GameManager.gameData.GetVersion(versionName);
        _chapter = version.chapters[chapterIndex];


        /// Chapter Name
        if (!String.IsNullOrEmpty(_chapter.chapterName))
            _chapterNameInputField.text = _chapter.chapterName;
        else
            _chapterNameInputField.text = "";

        var _customInputField = _chapterNameInputField.GetComponent<CustomInputField>();
        _customInputField.UpdateState();


        /// Chapter Image Name and Texture      
        if (!String.IsNullOrEmpty(_chapter.backgroundImageName))
        {
            /// check to load image from local drive or from url
            string fileUrl;
            string fileName;
            string temp = Data.imagesToUploadLocalPathList.Find(x => x == _chapter.backgroundImageOriginalPath);

            if (!String.IsNullOrEmpty(temp))
            {
                fileUrl = _chapter.backgroundImageOriginalPath;
                fileName = _chapter.backgroundImageOriginalPath;
            }
            else
            {
                fileUrl = Path.Combine(GameManager.userData.dataUrl, _chapter.backgroundImageName);
                fileName = _chapter.backgroundImageName;
            }

            /// update UI Name
            _backgroundImageNameText.text = fileName;

            /// update UI Texture
            FileDownloader fileDownloader = new FileDownloader();
            StartCoroutine(fileDownloader.LoadFileFromUrlToRawImage(fileUrl, _backgroundImage));
        }

        else
        {
            _backgroundImageNameText.text = "N/D";
            _backgroundImage.texture = null;
        }

        await Task.Yield();
        _isUpdatingUI = false;
    }

    public void OnSomeValueChanged()
    {
        if (_isUpdatingUI) return;

        _chapter.chapterName = _chapterNameInputField.text;
    }


    public async void OnClickOpen()
    {
        await Task.Delay(300);

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open FIle", "", "png", false);
        if (paths.Length > 0)
        {
            string fileUri = new System.Uri(paths[0]).AbsoluteUri;
            string filePath = new System.Uri(paths[0]).AbsolutePath;
            string fileName = Path.GetFileName(paths[0]);

            /// update UI Texture
            FileDownloader fileDownloader = new FileDownloader();
            StartCoroutine(fileDownloader.LoadFileFromUrlToRawImage(filePath, _backgroundImage));

            /// create and store the name of the image in gameData
            string id = System.Guid.NewGuid().ToString();
            _chapter.backgroundImageName = id + "-" + fileName;
            _chapter.backgroundImageOriginalPath = filePath;

            /// update UI text
            _backgroundImageNameText.text = _chapter.backgroundImageOriginalPath;

            /// add this image to the list of images to upload
            Data.imagesToUploadLocalPathList.Add(_chapter.backgroundImageOriginalPath);
            Data.imagesToUploadNameList.Add(_chapter.backgroundImageName);
        }
    }
}
