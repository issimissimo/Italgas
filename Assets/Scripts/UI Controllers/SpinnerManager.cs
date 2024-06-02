using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SpinnerManager : MonoBehaviour
{
    [SerializeField] CanvasController _canvasController;
    [SerializeField] TMP_Text _message;
    public bool isOn { get; private set; }

    void Awake()
    {
        _canvasController.SetOff();
    }

    public void OpenSpinner(string message = null)
    {
        print("OpenSpinner **************************************");
        if (message != null) _message.text = message;
        _message.gameObject.SetActive(String.IsNullOrEmpty(message) ? false : true);
        _canvasController.SetOn();
        isOn = true;
    }

    public void CloseSpinner()
    {
        print("CloseSpinner **************************************");
        _canvasController.SetOff();
        isOn = false;
    }
}
