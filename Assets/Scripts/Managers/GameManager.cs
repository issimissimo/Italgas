using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public bool isDevelopment;
    public float maximumSeconds = 10f;
    public bool sendMessageToRestart { get; set; } = false;
    public bool appJustStarted { get; set; } = true;
    public static float timer { get; private set; }
    public Globals.GAMESCENE gameScene { get; private set; }
    public static GameManager instance;
    private Coroutine _timerCoroutine;


    [SerializeField] ModalWindowManager _modalWindowManager;
    [SerializeField] NotificationManager _notificationManager;
    [SerializeField] SpinnerManager _spinnerManager;
    [SerializeField] GameObject _setupButton;
    [SerializeField] UiSetupZone _setupZone;


    /// Game DATA
    public static readonly string gameDataFileName = "Data.json";
    public static readonly float gameDefaultTimeInSeconds = 60f;
    public static Data.GameDataRoot gameData;


    /// Game SESSION DATA
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


    /// User DATA
    public static Data.UserData userData;


    /// Game Session DATA
    public static Data.GameSessionData gameSessionData;


    /// Player SCORE
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



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    //#region PUBLIC FUNCTIONS
    public void SetGameScene(Globals.GAMESCENE newGameScene)
    {
        if (_spinnerManager.isOn) CloseSpinner();

        // _setupButton.SetActive(newGameScene == Globals.GAMESCENE.CONFIG ? false : true);
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
        if (_spinnerManager.isOn) CloseSpinner();
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
    public void CloseSpinner()
    {
        _spinnerManager.CloseSpinner();
    }
    public void StartTimer(float seconds, Action callback = null)
    {
        if (_timerCoroutine != null) StopTimer();
        _timerCoroutine = StartCoroutine(TimerCoroutine(seconds, callback));
    }
    public void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }


    //#region PRIVATE FUNCTIONS
    private void InitializeSetupZone()
    {
        _setupZone.SetupUi();
    }
    private IEnumerator TimerCoroutine(float seconds, Action callback)
    {
        timer = 0f;
        while (timer < seconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (callback != null) callback.Invoke();
    }
}
