using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Michsky.UI.ModernUIPack;
using System.Collections.Generic;
using System.Collections;
using System;


public class GameManager : MonoBehaviour
{
    public bool isDevelopment;
    public bool sendMessageToRestart { get; set; } = false;
    public bool isAppJustStarted { get; set; } = true;
    public Globals.GAMESCENE gameScene { get; private set; }
    public static GameManager instance;


    [SerializeField] ModalWindowManager _modalWindowManager;
    [SerializeField] NotificationManager _notificationManager;
    [SerializeField] SpinnerManager _spinnerManager;
    [SerializeField] UiSetupZone _setupZone;
    [SerializeField] UiConnectionZone _connectionZone;


    [Space]
    [Header("Background images")]
    [SerializeField] Image _backgroundImage;
    [SerializeField] Sprite _defaultBackgroundSprite;
    [SerializeField] Background[] _backgrounds;



    //#region Game DATA
    public static readonly string gameDataFileName = "Data.json";
    public static readonly float gameDefaultTimeInSeconds = 60f;
    public static Data.GameDataRoot gameData;


    //#region Game SESSION DATA
    public static int currentGameChapterIndex = 0;
    public static int currentGamePageIndex = -1;
    public static Data.GameVersion currentGameVersion
    {
        get
        {
            Data.GameVersion version = gameData.GetVersion(gameData.currentVersion);
            return version;
        }
    }
    public static Data.GameChapter currentGameChapter
    {
        get
        {
            Data.GameChapter chapter = currentGameVersion.chapters[currentGameChapterIndex];
            return chapter;
        }
    }
    public static Data.GamePage currentGamePage
    {
        get
        {
            Data.GamePage page = currentGameChapter.pages[currentGamePageIndex];
            return page;
        }
    }


    //#region User DATA
    public static Data.UserData userData;


    //#region Game Session DATA
    public static Data.GameSessionData gameSessionData;


    //#region Player SCORE
    public class PlayerStats
    {
        public int totalQuestions;
        public int rightQuestions;
        public float totalTime;
        public int score;
        public PlayerStats(int playerId)
        {
            List<Data.SinglePlayerScore> singlePlayerScoreList = gameSessionData.scores[playerId].singlePlayerScoreList;

            totalQuestions = singlePlayerScoreList.Count;
            rightQuestions = 0;
            totalTime = 0f;
            foreach (var s in singlePlayerScoreList)
            {
                if (s.isCorrect) rightQuestions++;
                totalTime += s.timeSpent;
            }
            score = (rightQuestions * 100) - (int)totalTime;
        }
    }
    //#endregion

    //#region PRIVATE

    [Serializable]
    private class Background
    {
        public Globals.GAMEMODE gameMode;
        public Data.VERSION_NAME gameVersion;
        public Sprite sprite;
    }

    [Serializable]
    private class Connection
    {
        public Sprite sprite;
        public bool isActive;
    }


    private Coroutine _showSpinnerWithDelay;

    private IEnumerator ShowSpinnerWithDelayCoroutine(float delayTime, string description)
    {
        yield return new WaitForSeconds(delayTime);
        _showSpinnerWithDelay = null;
        ShowSpinner(description);
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        _backgroundImage.enabled = false;
    }

    private void InitializeSetupZone()
    {
        _setupZone.SetupUi();
    }





    //#region PUBLIC FUNCTIONS
    public void SetGameScene(Globals.GAMESCENE newGameScene)
    {
        CloseSpinner();

        _setupZone.gameObject.SetActive(newGameScene == Globals.GAMESCENE.CONFIG ? false : true);

        if (FindObjectOfType<NetworkManagerBase>() != null)
        {
            NetworkManagerBase networkManager = FindObjectOfType<NetworkManagerBase>();
            networkManager.Disconnect();
        }

        gameScene = newGameScene;
        SceneManager.LoadScene(gameScene.ToString());
    }
    public void Setup()
    {
        SetGameScene(Globals.GAMESCENE.CONFIG);
    }
    public void StartGame()
    {
        if (userData.gameMode == Globals.GAMEMODE.PLAYER) SetGameScene(Globals.GAMESCENE.PLAY);
        else if (userData.gameMode == Globals.GAMEMODE.VIEWER) SetGameScene(Globals.GAMESCENE.VIEW);
        InitializeSetupZone();
    }
    public void Restart()
    {
        SetGameScene(Globals.GAMESCENE.STARTUP);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void ShowModal(string title, string message, bool showConfigureButton, bool showRestartButton)
    {
        CloseSpinner();
        _modalWindowManager.OpenWindow(title, message, showConfigureButton, showRestartButton);
    }
    public void ShowNotification(string description)
    {
        _notificationManager.OpenNotification(description);
    }
    public void ShowSpinner(string description = null)
    {
        _spinnerManager.OpenSpinner(description);
    }
    public void ShowSpinner(float delayTime, string description = null)
    {
        _showSpinnerWithDelay = StartCoroutine(ShowSpinnerWithDelayCoroutine(delayTime, description));
    }
    public void CloseSpinner()
    {
        if (_showSpinnerWithDelay != null)
        {
            StopCoroutine(_showSpinnerWithDelay);
            _showSpinnerWithDelay = null;
        }
        else
        {
            if (_spinnerManager.isOn)
                _spinnerManager.CloseSpinner();
        }
    }
    public void SetBackground(bool useDefault = false)
    {
        CloseSpinner();

        Sprite sp = null;
        if (useDefault) sp = _defaultBackgroundSprite;
        else
        {
            foreach (var bck in _backgrounds)
            {
                if (bck.gameMode == userData.gameMode && bck.gameVersion == gameData.currentVersion)
                    sp = bck.sprite;
            }
        }
        if (sp != null)
        {
            _backgroundImage.enabled = true;
            _backgroundImage.sprite = sp;
        }
        else Debug.LogError("Non è stato inserito lo sprite per lo sfondo!");
    }
    public void OnUsersCountChanged(int users, int players)
    {
        print("-------------------------------");
        print("USERS: " + users + " --- PLAYERS: " + players);
        print("-------------------------------");

        if (userData.gameMode == Globals.GAMEMODE.PLAYER)
            _connectionZone.SetupUi(users, players);
    }



    //#region PRIVATE FUNCTIONS

}
