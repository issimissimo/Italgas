using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions.Tween;

public class testTween : MonoBehaviour
{
    public TweenPlayer anim;

    // Start is called before the first frame update
    void Start()
    {
        anim.enabled = false;
        // anim.onForwardArrived += arrived;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.onForwardArrived += arrived;
            anim.normalizedTime = 0f;
            anim.SetForwardDirectionAndEnabled();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            anim.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            anim.onForwardArrived += test;
            anim.normalizedTime = 0f;
            anim.SetForwardDirectionAndEnabled();
        }
    }

    void arrived()
    {
        print("oooooooooooooK");
    }

    void test()
    {
        print("testtttt");
    }

    public void Started(){
        print("STARTED!");
    }
}
