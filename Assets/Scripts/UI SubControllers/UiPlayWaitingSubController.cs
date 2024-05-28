using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayWaitingSubController : GamePanelSubControllerBase
{
    [SerializeField] Image _backgroundImage;
    [SerializeField] Sprite[] _backgroundTextures;

    public override void SetUI_on_STATE()
    {
        switch (GameManager.gameData.currentVersion)
        {
            case Data.VERSION_NAME.ADULTI:
                _backgroundImage.sprite = _backgroundTextures[0];
                break;

            case Data.VERSION_NAME.BAMBINI:
                _backgroundImage.sprite = _backgroundTextures[1];
                break;
        }
    }
}
