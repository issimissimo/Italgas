using System.IO;
using UnityEngine;
using System.Threading.Tasks;


public class ConfigManager : MonoBehaviour
{
    [SerializeField] CanvasController canvasController;
    [SerializeField] UiConfigUserDataController userDataController;
    [SerializeField] UiConfigGameDataController gameDataController;



    void Start()
    {
        canvasController.SetOn();
        userDataController.UpdateUI();
        gameDataController.UpdateUI();
    }

    /// <summary>
    /// BUTTON
    /// </summary>
    public void ContinueWithUserData()
    {
        GameManager.instance.StartGame();
    }


    /// <summary>
    /// BUTTON
    /// </summary>
    public void SaveUserDataAndRestart()
    {
        PlayerPrefs.SetString("gameMode", GameManager.userData.gameMode.ToString());
        PlayerPrefs.SetInt("playerId", GameManager.userData.playerId);
        PlayerPrefs.SetInt("requestedPlayers", GameManager.userData.requestedPlayers);
        PlayerPrefs.SetString("dataUrl", GameManager.userData.dataUrl);
        PlayerPrefs.SetString("ftpUserName", GameManager.userData.ftpUserName);
        PlayerPrefs.SetString("ftpPassword", GameManager.userData.ftpPassword);
        PlayerPrefs.SetString("ftpServer", GameManager.userData.ftpServer);
        PlayerPrefs.SetString("ftpFolder", GameManager.userData.ftpFolder);

        GameManager.instance.Restart();
    }


    


    /// <summary>
    /// BUTTON
    /// </summary>
    public void SaveGameData()
    {
        
        // GameManager.instance.ShowSpinner();
        GameManager.instance.ShowSpinner(delayTime: 1f);

        // await Task.Delay(500);

        if (Data.imagesToUploadLocalPathList.Count > 0)
        {
            UploadImages();
        }
        else
        {
            /// Save gameData
            string dataString = JsonUtility.ToJson(GameManager.gameData);
            string fileFullPath = Path.Combine(Application.persistentDataPath, GameManager.gameDataFileName);
            File.WriteAllText(fileFullPath, dataString);

            /// Upload the gameData to ftp
            FileUploader fileUploader = new FileUploader();

            CoroutineUtils.StartThrowingCoroutine(
            this,
            fileUploader.UploadToFTPCoroutine(fileFullPath, null, GameManager.userData.ftpServer, GameManager.userData.ftpUserName, 
            GameManager.userData.ftpPassword, GameManager.userData.ftpFolder),
            (ex) =>
            {
                if (ex != null)
                {
                    Debug.Log("Houson, we have a problem: " + ex);
                    string errorMessage = ex.ToString().Substring(0, 200);
                    GameManager.instance.ShowModal("ERRORE!", errorMessage, true, true);
                }
                else
                {
                    Debug.Log("UPLOADED!!!");

                    File.Delete(fileFullPath);

                    print("I DATI DI GIOCO SONO CAMBIATI, GLI ALTRI DEVONO RIAVVIARE!");


                    GameManager.instance.sendMessageToRestart = true;
                    GameManager.instance.Restart();
                }
            });
        }
    }


    /// <summary>
    /// UPLOAD THE IMAGES in Data.imagesToUploadLocalPathList
    /// </summary>
    private void UploadImages()
    {
        FileUploader fileUploader = new FileUploader();
        CoroutineUtils.StartThrowingCoroutine(
        this,
        fileUploader.UploadToFTPCoroutine(Data.imagesToUploadLocalPathList[0], Data.imagesToUploadNameList[0],
        GameManager.userData.ftpServer, GameManager.userData.ftpUserName, GameManager.userData.ftpPassword, GameManager.userData.ftpFolder),
        (ex) =>
        {
            if (ex != null)
            {
                Debug.Log("Houson, we have a problem: " + ex);
                string errorMessage = ex.ToString().Substring(0, 200);
                GameManager.instance.ShowModal("ERRORE!", errorMessage, true, true);
            }
            else
            {
                if (Data.imagesToUploadLocalPathList.Count > 0)
                {
                    Data.imagesToUploadLocalPathList.RemoveAt(0);
                    Data.imagesToUploadNameList.RemoveAt(0);
                }

                SaveGameData();
            }
        });
    }



    /// <summary>
    /// BUTTON
    /// </summary>
    public void QuitButton()
    {
        GameManager.instance.Quit();
    }


}
