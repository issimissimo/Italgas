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

    protected List<PlayerController> players = new List<PlayerController>();


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
        players.Clear();
        PlayerController[] playersArray = FindObjectsOfType<PlayerController>();
        players = playersArray.ToList();

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
