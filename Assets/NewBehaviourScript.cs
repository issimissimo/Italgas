using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LottiePlugin.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Material mat;

    public AnimatedImage[] animatedImage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Color c = new Color(1,1,1,0.5f);
            // mat.SetColor("_Color", c);

            foreach(var a in animatedImage) a.Play();
        }
    }
}
