using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TEST_ANIM : MonoBehaviour
{
    public UiAnimatedElement _answerAnimation;
    public List<UiAnimatedElement> _answerListAnimations;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var a in _answerListAnimations) a.Enter();

        StartCoroutine(Test());
    }

    // Update is called once per frame
    void Update()
    {
        // if (_answerAnimation.IsPlaying("Enter")){
        //     print("PLAYYYYYYYYYYY");
        // }



    }


    IEnumerator Test()
    {
        while (IsAnyAnimationPlaying(_answerListAnimations.ToArray(), "Enter"))
        {
            print("SI APRONO I TASTIIIIIIIIIIII");
            yield return null;
        }
        print("FINITO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }


    public bool IsAnyAnimationPlaying(UiAnimatedElement[] animations, string stateName)
    {
        var firstMatch = Array.Find(animations, elem => elem.IsPlaying(stateName) == true);
        return firstMatch == null ? true : false;
    }
}
