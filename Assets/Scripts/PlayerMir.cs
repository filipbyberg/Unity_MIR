using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class PlayerMir : MonoBehaviour
{
    float Offset = 500f;

    bool hasInitialized = false;

    // Update is called once per frame
    void Start()
    {
        float yPosition = 15;
        float xPosition = -15;

        Vector3 newPosition = new Vector3(xPosition + Offset, 0f, yPosition + Offset);
        transform.position = newPosition;

        hasInitialized = true;

        StartCoroutine(Polling("https://jiwa-api.azurewebsites.net/api/v1/mir_position"));
    }



    IEnumerator Polling(string uri)
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                        MirPositionClass mirPositionInfo = JsonUtility.FromJson<MirPositionClass>(webRequest.downloadHandler.text);

                        float yPosition = mirPositionInfo.status.position.x;
                        float xPosition = -mirPositionInfo.status.position.y;
                        float oriPosition = mirPositionInfo.status.position.orientation;
                        float linSpeed = mirPositionInfo.status.velocity.linear;
                        float angSpeed = mirPositionInfo.status.velocity.angular;

                        print("X: " + xPosition + ", Y: " + yPosition + ", Orientation: " + oriPosition + " , Linear speed: " + linSpeed + " , Angular speed: " +  angSpeed);

                        yield return MoveMir(xPosition, yPosition, oriPosition, linSpeed, angSpeed);

                        break;
                }
            }
        }
    }

    IEnumerator MoveMir(float xPosition, float yPosition, float oriPosition, float linSpeed, float angSpeed)
    {
        //print("X position: " + xPosition + " Y position: " + yPosition);
        linSpeed = linSpeed * 30f;
        angSpeed = angSpeed * 30f;

        Vector3 newPosition = new Vector3(xPosition + Offset, 0f, yPosition + Offset);
        Vector3 newRotation = new Vector3(0f, oriPosition, 0f);
        // Move the player towards the new position using linear speed
        Vector3 newPos = Vector3.MoveTowards(transform.position, newPosition, (linSpeed * Time.deltaTime) + 0.0001f);
        transform.position = newPos;

        // AngSpeed [0, 1]
        if (angSpeed >= 0 && angSpeed <= 1.0)
        {
            // Update rotation of the player
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, -oriPosition, (Time.deltaTime * angSpeed) + 0.25f);
            transform.eulerAngles = new Vector3(0f, angle, 0f);

            // Log the new position and other information
            //Debug.Log("Actual rot:" + angle);
            //Debug.Log("Actual pos: " + newPos);
            //Debug.Log("Linear1: " + linSpeed + " Angular:" + angSpeed + " X: " + xPosition + " Y: " + yPosition + " Orientation: " + oriPosition);
        }
        // AngSpeed [-1, 0] 
        else if (angSpeed >= -1.0 && angSpeed < 0)
        {
            // Update rotation of the player
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, -oriPosition, (Time.deltaTime * -angSpeed) + 0.25f); //Invert rotation
            transform.eulerAngles = new Vector3(0f, angle, 0f);

            // Log the new position and other information
            //Debug.Log("Actual rot:" + angle);
            //Debug.Log("Actual pos: " + newPos);
            //Debug.Log("Linear: " + linSpeed + " Angular:" + angSpeed + " X: " + xPosition + " Y: " + yPosition + " Orientation: " + oriPosition);
        }
        // AngSpeed [-inf, -1]
        else if (angSpeed < -1.0)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, -oriPosition, (Time.deltaTime * -angSpeed)); //Invert rotation
            transform.eulerAngles = new Vector3(0f, angle, 0f);

            // Log the new position and other information
            //Debug.Log("Actual rot:" + angle);
            //Debug.Log("Actual pos: " + newPos);
            //Debug.Log("Linear: " + linSpeed + " Angular:" + angSpeed + " X: " + xPosition + " Y: " + yPosition + " Orientation: " + oriPosition);
        }
        // AngSpeed [1, inf]
        else if (angSpeed > 1.0)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, -oriPosition, (Time.deltaTime * angSpeed));
            transform.eulerAngles = new Vector3(0f, angle, 0f);

            // Log the new position and other information
            //Debug.Log("Actual rot:" + angle);
            //Debug.Log("Actual pos: " + newPos);
            //Debug.Log("Linear: " + linSpeed + " Angular:" + angSpeed + " X: " + xPosition + " Y: " + yPosition + " Orientation: " + oriPosition);
        }

        yield return null;
    }
}
