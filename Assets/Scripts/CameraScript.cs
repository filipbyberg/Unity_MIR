using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject Camera1;
    public GameObject Camera2;
    public GameObject Camera3;
    //public GameObject Canvas;
    public int Manager;

  
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            CameraOne();
        }

        if (Input.GetKeyDown("2"))
        {
            CameraTwo();
        }

        if (Input.GetKeyDown("3"))
        {
            CameraThree();
        }



    }

    void CameraOne()
    {
        Camera1.SetActive(true);
        Camera2.SetActive(false);
        Camera3.SetActive(false);
        //Canvas.SetActive(true);
    }

    void CameraTwo()
    {
        Camera1.SetActive(false);
        Camera2.SetActive(true);
        Camera3.SetActive(false);
        //Canvas.SetActive(false);
    }

    void CameraThree()
    {
        Camera1.SetActive(false);
        Camera2.SetActive(false);
        Camera3.SetActive(true);
        //Canvas.SetActive(false);
    }
}
