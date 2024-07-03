using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class testPrefabs : MonoBehaviour
{
    [Serializable]
    public class TestKey
    {
        public KeyCode keyCode;
        public string animationName;
    }

    [Serializable]
    public class Animation
    {
        public AnimationsController animationsController;
        public TestKey[] testKeys;
    }


    public Animation[] animations;





    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                {
                    Debug.Log("KeyCode down: " + kcode);

                    foreach (var a in animations)
                    {
                        foreach (var t in a.testKeys)
                        {
                            if (t.keyCode == kcode)
                            {
                                a.animationsController.Tween_PlayByName(t.animationName);
                            }
                        }
                    }
                }
            }
        }






        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     animationsController.Tween_PlayByName("[ENTER WINNER]");
        // }
    }
}
