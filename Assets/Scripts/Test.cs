using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;

public class Test : MonoBehaviour
{
    [SerializeField] HorizontalSelector selector;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            selector.SetupItemsOnNumbers(5);
        }
    }
}
