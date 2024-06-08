using System;
using System.Collections.Generic;

public static class Data
{
    /// <summary>
    /// User Data
    /// </summary>

    public class UserData
    {
        public Globals.GAMEMODE gameMode { get; private set; }
        public int playerId { get; private set; }
        public int requestedPlayers { get; private set; }
        public string dataUrl { get; private set; }
        public string ftpUserName { get; private set; }
        public string ftpPassword { get; private set; }
        public string ftpServer { get; private set; }
        public string ftpFolder { get; private set; }
        public bool configurationComplete { get; private set; } = false;

        public void Set(Globals.GAMEMODE? gameMode = null, int? playerId = null, int? requestedPlayers = null, string dataUrl = null,
            string ftpUserName = null, string ftpPassword = null, string ftpServer = null, string ftpFolder = null, bool? configurationComplete = null)
        {
            if (gameMode != null) this.gameMode = gameMode.Value;
            if (playerId != null) this.playerId = playerId.Value;
            if (requestedPlayers != null) this.requestedPlayers = requestedPlayers.Value;
            if (dataUrl != null) this.dataUrl = dataUrl;
            if (ftpUserName != null) this.ftpUserName = ftpUserName;
            if (ftpPassword != null) this.ftpPassword = ftpPassword;
            if (ftpServer != null) this.ftpServer = ftpServer;
            if (ftpFolder != null) this.ftpFolder = ftpFolder;
            if (configurationComplete != null) this.configurationComplete = configurationComplete.Value;
        }
    }


    /// <summary>
    /// Keep track of new images to upload
    /// </summary>
    public static List<string> imagesToUploadLocalPathList;
    public static List<string> imagesToUploadNameList;


    /// <summary>
    /// Game Session Data
    /// </summary>
    public class SinglePlayerScore
    {
        public bool isCorrect;
        public float timeSpent;
        public int buttonPressed;
    }
    public class TotalPlayerScore
    {
        public List<SinglePlayerScore> singlePlayerScoreList;
    }
    public class GameSessionData
    {
        public int numberOfPlayersRunning;
        public TotalPlayerScore[] scores = new TotalPlayerScore[2];
    }


    /// <summary>
    /// Game Data
    /// </summary>

    public enum VERSION_NAME { ADULTI = 0, BAMBINI = 1 }


    [Serializable]
    public class GameDataRoot
    {
        public VERSION_NAME currentVersion;
        public List<GameVersion> versions;
        public GameVersion GetVersion(VERSION_NAME requiredVersionName)
        {
            foreach (var v in versions)
            {
                if (v.versionName == requiredVersionName)
                    return v;
            }
            return null;
        }
        public List<string> GetAllImages()
        {
            List<string> imageList = new List<string>();
            foreach (var v in versions)
            {
                foreach (var c in v.chapters)
                {
                    imageList.Add(c.backgroundImageName);
                }
            }
            return imageList;
        }
    }
    [Serializable]
    public class GameVersion
    {
        public VERSION_NAME versionName;
        public float maxTimeInSeconds;
        public List<GameChapter> chapters;
    }
    [Serializable]
    public class GameChapter
    {
        public string chapterName;
        public string backgroundImageName;
        public string backgroundImageOriginalPath;
        public List<GamePage> pages;
    }
    [Serializable]
    public class GamePage
    {
        public string question;
        public List<GameAnswer> answers;
    }
    [Serializable]
    public class GameAnswer
    {
        public string title;
        public bool isTrue;
    }
}
