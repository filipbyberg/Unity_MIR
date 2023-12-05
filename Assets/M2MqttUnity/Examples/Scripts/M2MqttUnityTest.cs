using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>

[Serializable]
public class VelocityData
{
    public float linear;
    public float angular;
}

[Serializable]
public class PositionData
{
    public float y;
    public float x;
    public float orientation;
}

[Serializable]
public class ReceivedMessage
{
    public VelocityData velocity;
    public PositionData position;
}

/// <summary>
/// Script for testing M2MQTT with a Unity UI
/// </summary>
public class M2MqttUnityTest : M2MqttUnityClient
{
    
    [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
    public bool autoTest = false;
    [Header("User Interface")]
    public InputField consoleInputField;
    public Toggle encryptedToggle;
    public InputField addressInputField;
    public InputField portInputField;
    public Button connectButton;
    public Button disconnectButton;
    public Button testPublishButton;
    public Button clearButton;

    private List<string> eventMessages = new List<string>();
    private bool updateUI = false;


    public void TestPublish()
    {
        client.Publish("192.168.145.23/mir/robot1/test", System.Text.Encoding.UTF8.GetBytes("Test message"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Test message published");
        AddUiMessage("Test message published.");
    }

    public void SetBrokerAddress(string brokerAddress)
    {
        if (addressInputField && !updateUI)
        {
            this.brokerAddress = brokerAddress;
        }
    }

    public void SetBrokerPort(string brokerPort)
    {
        if (portInputField && !updateUI)
        {
            int.TryParse(brokerPort, out this.brokerPort);
        }
    }

    public void SetEncrypted(bool isEncrypted)
    {
        this.isEncrypted = isEncrypted;
    }


    public void SetUiMessage(string msg)
    {
        if (consoleInputField != null)
        {
            consoleInputField.text = msg;
            updateUI = true;
        }
    }

    public void AddUiMessage(string msg)
    {
        if (consoleInputField != null)
        {
            consoleInputField.text += msg + "\n";
            updateUI = true;
        }
    }

    protected override void OnConnecting()
    {
        base.OnConnecting();
        SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
    }

    protected override void OnConnected()
    {
        base.OnConnected();
        SetUiMessage("Connected to broker on " + brokerAddress + "\n");

        if (autoTest)
        {
            TestPublish();
        }
    }

    protected override void SubscribeTopics()
    {
        client.Subscribe(new string[] { "192.168.145.23/mir/robot1/data" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    protected override void UnsubscribeTopics()
    {
        client.Unsubscribe(new string[] { "192.168.145.23/mir/robot1/data" });
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        AddUiMessage("CONNECTION FAILED! " + errorMessage);
    }

    protected override void OnDisconnected()
    {
        AddUiMessage("Disconnected.");
    }

    protected override void OnConnectionLost()
    {
        AddUiMessage("CONNECTION LOST!");
    }

    private void UpdateUI()
    {
        if (client == null)
        {
            if (connectButton != null)
            {
                connectButton.interactable = true;
                disconnectButton.interactable = false;
                testPublishButton.interactable = false;
            }
        }
        else
        {
            if (testPublishButton != null)
            {
                testPublishButton.interactable = client.IsConnected;
            }
            if (disconnectButton != null)
            {
                disconnectButton.interactable = client.IsConnected;
            }
            if (connectButton != null)
            {
                connectButton.interactable = !client.IsConnected;
            }
        }
        if (addressInputField != null && connectButton != null)
        {
            addressInputField.interactable = connectButton.interactable;
            addressInputField.text = brokerAddress;
        }
        if (portInputField != null && connectButton != null)
        {
            portInputField.interactable = connectButton.interactable;
            portInputField.text = brokerPort.ToString();
        }
        if (encryptedToggle != null && connectButton != null)
        {
            encryptedToggle.interactable = connectButton.interactable;
            encryptedToggle.isOn = isEncrypted;
        }
        if (clearButton != null && connectButton != null)
        {
            clearButton.interactable = connectButton.interactable;
        }
        updateUI = false;
    }

    protected override void Start()
    {
        SetUiMessage("Ready.");
        updateUI = true;
        base.Start();
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        string msg = System.Text.Encoding.UTF8.GetString(message);
        Debug.Log("Received: " + msg);
        StoreMessage(msg);
        if (topic == "192.168.145.23/mir/robot1/data")
        {
            if (autoTest)
            {
                autoTest = false;
                Disconnect();
            }
        }
    }

    private void StoreMessage(string eventMsg)
    {
        eventMessages.Add(eventMsg);
    }

    public float XPosition { get; private set; }
    public float YPosition { get; private set; }
    public float OriPosition { get; private set; }
    public float LinSpeed { get; private set; }
    public float AngSpeed { get; private set; }

    public void ProcessMessage(string msg)
    {
        //AddUiMessage("Received: " + msg);
        try
        {
            ReceivedMessage receivedMessage = JsonUtility.FromJson<ReceivedMessage>(msg);

            float linear = receivedMessage.velocity.linear;
            float angular = receivedMessage.velocity.angular;
            float y = receivedMessage.position.y;
            float x = receivedMessage.position.x;
            float orientation = receivedMessage.position.orientation;

            // Update the public properties with the new values
            XPosition = x;
            YPosition = y;
            OriPosition = orientation;
            LinSpeed = linear;
            AngSpeed = angular;

            //playerObject.transform.position = new Vector3(y+500, 0f, x+500);
            //playerObject.transform.eulerAngles = new Vector3(0f, orientation, 0f);

            AddUiMessage("Linear: " + linear + ", Angular: " + angular + ", X: " + x + ", Y: " + y + ", O: " + orientation);
        }
        catch (Exception e)
        {
            Debug.LogError("Error parsing JSON: " + e.Message);
        }
    }

    protected override void Update()
    {
        base.Update(); // call ProcessMqttEvents()

        if (eventMessages.Count > 0)
        {
            foreach (string msg in eventMessages)
            {
                ProcessMessage(msg);
            }
            eventMessages.Clear();
        }
        if (updateUI)
        {
            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnValidate()
    {
        if (autoTest)
        {
            autoConnect = true;
        }
    }
}

