using UnityEngine;
using UnityEngine.SceneManagement;
using Michsky.UI.ModernUIPack;
using System.Collections.Generic;
using System.Collections;
using System;



public class GameManager : MonoBehaviour
{
    //#region INSPECTOR

    [Header("SETTINGS")]
    public bool isDevelopment;
    [SerializeField] bool _singlePageMode;
    [SerializeField] bool _showBetaImage;
    [field: SerializeField] public int FusionDelayTime { get; private set; } = 500;


    [Space]
    [Header("UI")]
    [SerializeField] ModalWindowManager _modalWindowManager;
    [SerializeField] NotificationManager _notificationManager;
    [SerializeField] SpinnerManager _spinnerManager;
    [SerializeField] UiSetupZone _setupZone;
    [SerializeField] UiConnectionZone _connectionZone;
    [SerializeField] GameObject _betaImage;


    [Space]
    [SerializeField] Background[] _backgroundPanels;


    [Space]
    [Header("Common audio clips")]
    [SerializeField] AudioSource _clickAudioSource;
    [SerializeField] AudioSource _softClickAudioSource;
    [SerializeField] AudioSource _superSoftClickAudioSource;

    //#endregion



    public bool sendMessageToRestart { get; set; } = false;
    public bool isAppJustStarted { get; set; } = true;
    public Globals.GAMESCENE gameScene { get; private set; }
    public static GameManager instance;




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
        public float score;
        public PlayerStats(int playerId)
        {
            List<Data.SinglePlayerScore> singlePlayerScoreList = gameSessionData.scores[playerId].singlePlayerScoreList;

            totalQuestions = singlePlayerScoreList.Count;
            rightQuestions = 0;

            totalTime = 0f;
            score = 0f;
            foreach (var s in singlePlayerScoreList)
            {
                if (s.isCorrect)
                {
                    rightQuestions++;
                    score += SingleScore(totalQuestions, s.timeSpent);
                }

                totalTime += s.timeSpent;
            }

            float SingleScore(int totalQuestions, float timeSpent)
            {
                float minPenalty = 0f;
                float maxPenalty = 100f / totalQuestions / 2;

                float penaltyPercent = Mathf.InverseLerp(0f, currentGameVersion.maxTimeInSeconds, timeSpent);
                float penalty = Mathf.Lerp(minPenalty, maxPenalty, penaltyPercent);

                float score = (100f / totalQuestions) - penalty;
                return score;
            }
        }
    }



    //#region PRIVATE

    [Serializable]
    private class Background
    {
        public Globals.GAMEMODE gameMode;
        // public Data.VERSION_NAME gameVersion;
        public GameObject panel;
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

        foreach (var bck in _backgroundPanels)
            bck.panel.SetActive(false);
    }

    void Start()
    {
        _betaImage.SetActive(_showBetaImage);
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
        _connectionZone.gameObject.SetActive(newGameScene == Globals.GAMESCENE.CONFIG ? false : true);

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
    public void SetBackground()
    {
        CloseSpinner();

        foreach (var bck in _backgroundPanels)
            bck.panel.SetActive(bck.gameMode == userData.gameMode ? true : false);
    }
    public void SetupConnectionZone(int users, int players)
    {
        _connectionZone.SetupUi(userData.gameMode, users, players);
    }
    public void PlayAudioClick()
    {
        _clickAudioSource.Play();
    }
    public void PlayAudioSoftClick()
    {
        _softClickAudioSource.Play();
    }
    public void PlayAudioSuperSoftClick()
    {
        _superSoftClickAudioSource.Play();
    }


    //#region GAME LOGICS

    public enum GAME_STATE { CHAPTER, PAGE, END }
    public void GetNewGameState(Action<GAME_STATE> callback)
    {
        if (currentGamePageIndex <= currentGameChapter.pages.Count - 1)
        {
            if (currentGamePageIndex == 0) callback(GAME_STATE.CHAPTER);
            else
            {
                if (_singlePageMode) callback(GAME_STATE.END);
                else callback(GAME_STATE.PAGE);
            }
        }
        else
        {
            if (currentGameChapterIndex < currentGameVersion.chapters.Count - 1)
            {
                /// Iterate
                currentGameChapterIndex++;
                currentGamePageIndex = 0;
                GetNewGameState(callback);
            }
            else callback(GAME_STATE.END);
        }
    }

    //#endregion



    [Serializable]
    public struct ExitTime
    {
        public string stateName;
        public float exitTime;
    }
    [Space]
    [Header("Shared Exit times (Tablet/PC)")]
    public ExitTime[] exitTimes;



    public float GetStateExitTime(string stateName)
    {
        foreach (var xt in exitTimes)
            if (xt.stateName == stateName)
            {
                if (xt.exitTime == 0f)
                {
                    Debug.LogError("Exit Time for state - " + stateName + " - is Zero!");
                    return 999;
                }
                return xt.exitTime;
            }
        return 0;
    }

}
