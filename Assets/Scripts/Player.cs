using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public M2MqttUnityTest mqttUnityTest; // Reference to the M2MqttUnityTest script

    bool hasInitialized = false; // Flag to track if initialization has occurred

    float Offset = 500f;

    private void Update()
    {
        // MQTT
        if (mqttUnityTest != null)
        {
            // Extract properties from mqttUnityTest instance
            float yPosition = mqttUnityTest.XPosition;
            float xPosition = -mqttUnityTest.YPosition;
            float oriPosition = mqttUnityTest.OriPosition;
            float linSpeed = mqttUnityTest.LinSpeed;
            float angSpeed = mqttUnityTest.AngSpeed;

            // Calculate new position and rotation for the player
            Vector3 newPosition = new Vector3(xPosition + Offset, 0f, yPosition + Offset);
            Vector3 newRotation = new Vector3(0f, oriPosition, 0f);

            // Check if not initialized and xPosition is not zero
            if (!hasInitialized && yPosition != 0)
            {
                // Set the initial position and rotation of the player
                transform.position = newPosition;
                transform.eulerAngles = newRotation;

                // Update the flag to indicate initialization
                hasInitialized = true;
            }
            else
            {
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
                    Debug.Log("Actual rot:" + angle);
                    Debug.Log("Actual pos: " + newPos);
                    Debug.Log("Linear1: " + linSpeed + " Angular:" + angSpeed + " X: " + xPosition + " Y: " + yPosition + " Orientation: " + oriPosition);
                }
                // AngSpeed [-1, 0] 
                else if (angSpeed >= -1.0 && angSpeed < 0)
                {
                    // Update rotation of the player
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, -oriPosition, (Time.deltaTime * -angSpeed) + 0.25f); //Invert rotation
                    transform.eulerAngles = new Vector3(0f, angle, 0f);

                    // Log the new position and other information
                    Debug.Log("Actual rot:" + angle);
                    Debug.Log("Actual pos: " + newPos);
                    Debug.Log("Linear: " + linSpeed + " Angular:" + angSpeed + " X: " + xPosition + " Y: " + yPosition + " Orientation: " + oriPosition);
                }
                // AngSpeed [-inf, -1]
                else if (angSpeed < -1.0)
                {
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, -oriPosition, (Time.deltaTime * -angSpeed)); //Invert rotation
                    transform.eulerAngles = new Vector3(0f, angle, 0f);

                    // Log the new position and other information
                    Debug.Log("Actual rot:" + angle);
                    Debug.Log("Actual pos: " + newPos);
                    Debug.Log("Linear: " + linSpeed + " Angular:" + angSpeed + " X: " + xPosition + " Y: " + yPosition + " Orientation: " + oriPosition);
                }
                // AngSpeed [1, inf]
                else if (angSpeed > 1.0)
                {
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, -oriPosition, (Time.deltaTime * angSpeed));
                    transform.eulerAngles = new Vector3(0f, angle, 0f);

                    // Log the new position and other information
                    Debug.Log("Actual rot:" + angle);
                    Debug.Log("Actual pos: " + newPos);
                    Debug.Log("Linear: " + linSpeed + " Angular:" + angSpeed + " X: " + xPosition + " Y: " + yPosition + " Orientation: " + oriPosition);
                }

                //float rotatespeed = 1f;
                //transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, newRotation, Time.deltaTime*rotatespeed);


            }
        }

    }
}
