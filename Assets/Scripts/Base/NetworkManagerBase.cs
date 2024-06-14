using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Fusion;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public abstract class NetworkManagerBase : MonoBehaviour
{
    [SerializeField] Globals.GAMESCENE _gameSceneName; /// set in inspector the name of this manager
    [SerializeField] GameObject PrototypeNetworkStartPrefab;
    [SerializeField] protected UiController[] _uiControllers;

    public List<PlayerController> players = new List<PlayerController>();
    public PlayerController myPlayer { get; protected set; } = null;


    IEnumerator Start()
    {
        if (GameManager.instance.gameScene == _gameSceneName)
        {
            /// Instantiate the Fusion Runner
            Instantiate(PrototypeNetworkStartPrefab, Vector3.zero, Quaternion.identity);

            /// Subscribe to network events
            NetworkEventsDispatcher.OnConnectFailed += OnConnectFailed;
            NetworkEventsDispatcher.OnPlayerJoined += OnPlayerJoined;
            NetworkEventsDispatcher.OnPlayerLeft += OnPlayerLeft;

            /// Get the Runner
            yield return new WaitForSeconds(0.5f);

            Started();
        }

        /// Destroy this scene that is unnecessary created (I don't know why)
        else SceneManager.UnloadSceneAsync(_gameSceneName.ToString());
    }


    private void OnDisable()
    {
        NetworkEventsDispatcher.OnConnectFailed -= OnConnectFailed;
        NetworkEventsDispatcher.OnPlayerJoined -= OnPlayerJoined;
        NetworkEventsDispatcher.OnPlayerLeft -= OnPlayerLeft;
    }

    protected virtual void Started()
    {
        /// to override
    }

    private void OnConnectFailed()
    {
        GameManager.instance.ShowModal("MANCATA CONNESSIONE", "Non è stato possibile connettersi al Server", showConfigureButton: false, showRestartButton: false);
    }

    private async void OnPlayerLeft()
    {
        await Task.Delay(500);
        OnRealPlayersCountChanged();

        SetupUi();
    }

    private void OnPlayerJoined()
    {
        SetupUi();
    }


    private void SetupUi()
    {
        /// Tell the GameManager how many connected users and real players
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        GameManager.instance.SetupConnectionZone(runner.ActivePlayers.Count(), players.Count);
    }



    /// <summary>
    /// CHECK FOR PLAYERS (ONLY REAL PLAYERS, NOT THE VIEWER) COUNT CHANGE
    /// This happens only when someone shutdown or start
    /// </summary>
    public virtual void OnRealPlayersCountChanged()
    {
        players.Clear();
        PlayerController[] playersArray = FindObjectsOfType<PlayerController>();
        players = playersArray.ToList();


        /// Tell the GameManager how many connected users and real players
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        GameManager.instance.SetupConnectionZone(runner.ActivePlayers.Count(), players.Count);

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

}
