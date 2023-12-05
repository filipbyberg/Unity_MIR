using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public GameObject Camera1;
    public GameObject Camera2;
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



    }

    void CameraOne()
    {
        Camera1.SetActive(true);
        Camera2.SetActive(false);
    }

    void CameraTwo()
    {
        Camera1.SetActive(false);
        Camera2.SetActive(true);
    }
}
