using UnityEngine;
using UnityEngine.UI;

public class UiConnectionZone : MonoBehaviour
{
    [SerializeField] private Image _notConnectedImage;


    void Start()
    {
        _notConnectedImage.color = new Color(1, 1, 1, 0);
    }

    public void SetupUi(Globals.GAMEMODE gameMode, int users, int players)
    {
        bool isScreenConnected = users > players ? true : false;

        if (isScreenConnected || gameMode == Globals.GAMEMODE.VIEWER)
        {
            _notConnectedImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            _notConnectedImage.color = new Color(1, 1, 1, 1);
        }
    }
}
