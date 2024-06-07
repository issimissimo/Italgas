using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiSetupZone : MonoBehaviour
{
    [SerializeField] private Image[] _playerNumberImages;

    private Coroutine _openSetupCoroutine;


    void Start()
    {
        /// Hide all images
        for (int i = 0; i < _playerNumberImages.Length; i++)
        {
            Image img = _playerNumberImages[i];
            img.color = new Color(img.color.r, img.color.r, img.color.g, 0.3f);
        }
    }

    public void SetupUi()
    {
        /// playerNumberImages
        for (int i = 0; i < _playerNumberImages.Length; i++)
        {
            float alpha = 0.3f;

            if (GameManager.userData.gameMode == Globals.GAMEMODE.PLAYER)
            {
                alpha = i == GameManager.userData.playerId ? 1f : 0.3f;
            }

            Image img = _playerNumberImages[i];
            img.color = new Color(img.color.r, img.color.r, img.color.g, alpha);
        }
    }


    public void OnPointerDown()
    {
        print("DDDDDDDDDDDDDDDD");

        if (_openSetupCoroutine != null) StopCoroutine(_openSetupCoroutine);
        _openSetupCoroutine = StartCoroutine(OpenSetupCoroutine());
    }
    public void OnPointerUp()
    {
        if (_openSetupCoroutine != null) StopCoroutine(_openSetupCoroutine);
    }

    /// <summary>
    /// BUTTON
    /// </summary>
    public void OpenSetup()
    {
        StartCoroutine(OpenSetupCoroutine());
    }

    private IEnumerator OpenSetupCoroutine()
    {
        float time = 0f;
        float maxTime = 3f;
        while (time < maxTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        /// Open Setup
        GameManager.instance.Setup();
    }
}
