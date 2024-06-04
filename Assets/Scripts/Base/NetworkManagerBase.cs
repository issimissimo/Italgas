using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Fusion;
using System.Collections.Generic;
using System.Linq;


public abstract class NetworkManagerBase : MonoBehaviour
{
    [SerializeField] Globals.GAMESCENE _gameSceneName; /// set in inspector the name of this manager
    [SerializeField] GameObject PrototypeNetworkStartPrefab;


    [SerializeField] protected UiControllerBase[] _uiControllers;

    protected List<PlayerController> _players = new List<PlayerController>();
    public PlayerController _myPlayer { get; protected set; } = null;



    protected async void Start()
    {
        if (GameManager.instance.gameScene == _gameSceneName)
        {
            /// Instantiate the Fusion Runner
            Instantiate(PrototypeNetworkStartPrefab, Vector3.zero, Quaternion.identity);

            /// Subscribe to network events
            NetworkEventsDispatcher.OnConnectFailed += OnConnectFailed;
            NetworkEventsDispatcher.OnPlayerLeft += OnPlayerLeft;

            // /// Subscribe to GameManager
            // GameManager.instance.OnTimeRunningFinished += OnTimeFinished;

            /// Get the Runner
            await Task.Delay(500);

            Started();
        }

        /// Destroy this scene that is unnecessary created (I don't know why)
        else SceneManager.UnloadSceneAsync(_gameSceneName.ToString());
    }


    private void OnDisable()
    {
        NetworkEventsDispatcher.OnConnectFailed -= OnConnectFailed;
        NetworkEventsDispatcher.OnPlayerLeft -= OnPlayerLeft;
        // GameManager.instance.OnTimeRunningFinished -= OnTimeFinished;
    }

    public virtual void Started()
    {
        /// to override
    }

    private void OnConnectFailed()
    {
        GameManager.instance.ShowModal("ERRORE DI CONNESSIONE", "Non è stato possibile connettersi al Server", showConfigureButton: false, showRestartButton: false);
    }

    private async void OnPlayerLeft()
    {
        await Task.Delay(500);
        OnPlayersCountChanged();
    }

    public virtual void OnPlayersCountChanged()
    {
        print("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
        _players.Clear();
        PlayerController[] playersArray = FindObjectsOfType<PlayerController>();
        _players = playersArray.ToList();

        /// to be implemented
    }

    public virtual void OnPlayerStateChanged(int playerId, PlayerController.STATE state)
    {
        /// to override
    }

    public virtual void OnPlayerRunningStateChanged(int playerId, PlayerController.RUNNING_STATE runningState)
    {
        /// to override
    }


    public void Disconnect()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        runner.Shutdown();

        /// dirty solution to remove the Runner...
        NetworkRunner[] r = FindObjectsOfType<NetworkRunner>();
        foreach (var aa in r) Destroy(aa.gameObject);
        FusionBootstrap[] f = FindObjectsOfType<FusionBootstrap>();
        foreach (var aa in f) Destroy(aa.gameObject);
    }





    /// <summary>
    /// 
    /// </summary>
    protected void ProceedToNext()
    {
        if (GameManager.currentGamePageIndex <= GameManager.currentGameChapter.pages.Count - 1)
        {
            if (GameManager.currentGamePageIndex == 0)
            {
                /// Open a Chapter, and next open a Page
                foreach (var ui in _uiControllers) ui.Set_RUNNING_STATE_OPEN_CHAPTER(() => ui.Set_RUNNING_STATE_OPEN_PAGE());
            }
            else
            {
                /// Open a Page
                foreach (var ui in _uiControllers) ui.Set_RUNNING_STATE_OPEN_PAGE();
            }
        }
        else
        {
            if (GameManager.currentGameChapterIndex < GameManager.currentGameVersion.chapters.Count - 1)
            {
                /// Iterate
                GameManager.currentGameChapterIndex++;
                GameManager.currentGamePageIndex = 0;
                ProceedToNext();
            }
            else
            {
                /// Finished
                // GameManager.currentGameChapterIndex = 0;
                // GameManager.currentGamePageIndex = -1;

                print("------------ FINITO!!!!!!!!!!!");

                
                // _myPlayer.NetworkedState = PlayerController.STATE.FINISHED;

                /// Player
                if (_myPlayer != null) _myPlayer.Set_RUNNING_STATE_NONE();

                /// Viewer
                else foreach(var p in _players) p.Set_RUNNING_STATE_NONE();
                

                // /// UI
                // foreach (var ui in _uiControllers) ui.Set_RUNNING_STATE_FINAL_SCORE();
            }
        }
    }

    /// <summary>
    /// /////////////////////////////// TESTTTTTTTTT
    /// </summary>
    // protected void ProceedToNext()
    // {
    //     if (GameManager.currentGamePageIndex == 0)
    //     {
    //         foreach (var ui in _uiControllers) ui.Set_RUNNING_STATE_OPEN_CHAPTER(()=> ui.Set_RUNNING_STATE_OPEN_PAGE());
    //         // foreach (var ui in _uiControllers) ui.Set_RUNNING_STATE_OPEN_CHAPTER(()=> {});


    //     }
    //     else
    //     {
    //         print("------------ FINITO!!!!!!!!!!!");

    //         /// Game is finished!!!
    //         GameManager.currentGameChapterIndex = 0;
    //         GameManager.currentGamePageIndex = -1;

    //         /// Set my Player
    //         // _myPlayer.NetworkedState = PlayerController.STATE.FINISHED;
    //         if (_myPlayer != null) _myPlayer.Set_RUNNING_STATE_NONE();

    //         /// UI
    //         foreach (var ui in _uiControllers) ui.Set_RUNNING_STATE_FINAL_SCORE();

    //     }
    // }

}
