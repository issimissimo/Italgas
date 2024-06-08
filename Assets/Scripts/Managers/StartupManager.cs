using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;

public class StartupManager : MonoBehaviour
{
    /// <summary>
    /// START
    /// </summary>
    private void Start()
    {
        StartUp();
    }


    private void StartUp()
    {
        GameManager.instance.CloseSpinner();
        GameManager.instance.ShowSpinner(delayTime: 1f);

        LoadUserData((success) =>
        {
            if (success)
            {
                print("NOW WE SHOULD LOAD THE GAME DATA...");
                LoadGameData((success) =>
                                {
                                    if (success)
                                    {
                                        /// Start the Game
                                        print("NOW WE SHOULD START THE GAME...");

                                        GameManager.instance.SetBackground();

                                        if (GameManager.instance.isDevelopment)
                                        {
                                            print("TUTTO OK!!! MA SICCOME SEI IN DEVELOPMENT, APRO COMUNQUE LA CONFIGURAZIONE");
                                            GameManager.instance.Setup();
                                        }
                                        else
                                        {
                                            print("LAUNCH THE GAME!");
                                            GameManager.instance.StartGame();
                                        }
                                    }
                                    else GameManager.instance.SetBackground(useDefault: true);
                                });
            }
            else GameManager.instance.SetBackground(useDefault: true);
        });
    }



    /// <summary>
    /// LOAD USER DATA
    /// </summary>
    /// <param name="callback"></param>
    private void LoadUserData(Action<bool> callback)
    {
        GameManager.userData = new Data.UserData();

        string gameMode = PlayerPrefs.GetString("gameMode");

        Globals.GAMEMODE userGameMode = String.IsNullOrEmpty(gameMode) ?
            Globals.GAMEMODE.PLAYER :
            (Globals.GAMEMODE)System.Enum.Parse(typeof(Globals.GAMEMODE), gameMode);

        GameManager.userData.Set(
                gameMode: userGameMode,
                dataUrl: PlayerPrefs.GetString("dataUrl"),
                playerId: PlayerPrefs.GetInt("playerId"),
                requestedPlayers: PlayerPrefs.GetInt("requestedPlayers"),
                ftpUserName: PlayerPrefs.GetString("ftpUserName"),
                ftpPassword: PlayerPrefs.GetString("ftpPassword"),
                ftpServer: PlayerPrefs.GetString("ftpServer"),
                ftpFolder: PlayerPrefs.GetString("ftpFolder")
                );


        if (String.IsNullOrEmpty(GameManager.userData.dataUrl))
        {
            GameManager.instance.ShowModal("ATTENZIONE", "Non è stato impostato il percorso dei file in rete su questo dispositivo.\nProcedi a configurare le impostazioni utente",
                showConfigureButton: true, showRestartButton: false);

            callback(false);
        }
        else
        {
            if (GameManager.userData.gameMode == Globals.GAMEMODE.VIEWER)
            {
                callback(true);
            }
            else
            {
                if (GameManager.userData.requestedPlayers == 0 || String.IsNullOrEmpty(GameManager.userData.ftpUserName) || String.IsNullOrEmpty(GameManager.userData.ftpPassword) ||
                String.IsNullOrEmpty(GameManager.userData.ftpServer) || String.IsNullOrEmpty(GameManager.userData.ftpFolder))
                {
                    GameManager.instance.ShowModal("ATTENZIONE", "La configurazione è incompleta su questo dispositivo.\nProcedi a configurare le impostazioni utente",
                        showConfigureButton: true, showRestartButton: false);

                    callback(false);
                }
                else
                {
                    GameManager.userData.Set(configurationComplete: true);
                    callback(true);
                }
            }
        }
    }


    /// <summary>
    /// LOAD GAME DATA
    /// </summary>
    /// <param name="callback"></param>
    private void LoadGameData(Action<bool> callback)
    {
        GameManager.gameData = new Data.GameDataRoot();

        FileDownloader fileDownloader = new FileDownloader();
        StartCoroutine(fileDownloader.LoadTextFromUrl(Path.Combine(GameManager.userData.dataUrl, GameManager.gameDataFileName), (result) =>
        {
            switch (result.state)
            {
                case FileDownloader.STATE.SUCCESS:
                    GameManager.gameData = JsonUtility.FromJson<Data.GameDataRoot>(result.downloadedText);

                    if (GameManager.userData.gameMode == Globals.GAMEMODE.VIEWER)
                    {
                        StartCoroutine(LoadImages((result) =>
                        {
                            if (!result)
                            {
                                GameManager.instance.ShowModal("ERRORE", "Non sono state trovate le immagini da scaricare o si è verificato un problema di rete", false, true);
                            }
                            callback(result);
                        }));
                    }
                    else if (GameManager.userData.gameMode == Globals.GAMEMODE.PLAYER)
                    {
                        callback(true);
                    }
                    break;

                case FileDownloader.STATE.NOT_FOUND:

                    ///
                    /// Initialize gameData
                    ///
                    GameManager.gameData.versions = new List<Data.GameVersion>();

                    /// Add Versions
                    Data.GameVersion version;
                    foreach (Data.VERSION_NAME name in Enum.GetValues(typeof(Data.VERSION_NAME)))
                    {
                        version = new Data.GameVersion
                        {
                            versionName = name,
                            maxTimeInSeconds = GameManager.gameDefaultTimeInSeconds,
                            chapters = new List<Data.GameChapter>()
                        };

                        /// Add 3 Chapters with 1 Page
                        Data.GameChapter chapter;
                        Data.GamePage page;
                        for (int i = 0; i < 3; i++)
                        {
                            page = new Data.GamePage
                            {
                                answers = new List<Data.GameAnswer>()
                            };
                            chapter = new Data.GameChapter
                            {
                                pages = new List<Data.GamePage>()
                            };
                            chapter.pages.Add(page);
                            version.chapters.Add(chapter);
                        }

                        GameManager.gameData.versions.Add(version);
                    }

                    GameManager.instance.ShowModal("ATTENZIONE", "Non sonoi stati trovati i dati di gioco.\nProcedi a crearli nelle impostazioni.",
                    showConfigureButton: true, showRestartButton: false);
                    callback(false);
                    break;

                case FileDownloader.STATE.NETWORK_ERROR:
                    GameManager.instance.ShowModal("ERRORE", "Ci sono problemi di rete!\nVerifica la tua connessione.",
                    showConfigureButton: false, showRestartButton: false);
                    callback(false);
                    break;
            }
        }));
    }

    private IEnumerator LoadImages(Action<bool> callback)
    {
        FileDownloader fileDownloader = new FileDownloader();
        List<string> imageList = GameManager.gameData.GetAllImages();
        bool success = true;
        foreach (var st in imageList)
        {
            string localFilePath = Path.Combine(Application.persistentDataPath, st);
            if (!string.IsNullOrEmpty(st) && !File.Exists(localFilePath))
            {
                string remotePath = GameManager.userData.dataUrl + "/" + st;
                print("DEVO SCARICARE: " + remotePath);
                yield return fileDownloader.DownloadAndSave(localFilePath, remotePath, (result) =>
                {
                    if (result.state != FileDownloader.STATE.SUCCESS)
                    {
                        success = false;
                    }
                });

            }
        }
        callback(success);
    }
}


