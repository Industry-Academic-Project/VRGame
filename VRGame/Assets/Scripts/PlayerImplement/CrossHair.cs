using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public Transform crosshair;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ARAVRInput.DrawCrosshair(crosshair);
        
        //Shot(); // shot 코드를 작성해야함
    }

    // void Shot()
    // {
    //     
    // }
}
