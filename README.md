## 3d Digital Twin for MIR-robot in Unity

![image](https://github.com/filipbyberg/Unity_MIR/assets/80341025/059629ba-7280-4e4f-96b9-02e5d5a6b10a)

## What is it?

This is a Unity project Digital Twin (DT) for a MIR robot operating at a warehouse. It takes real time data from the actual MIR robot to mimic the movements of the virtual robot.

## Showcases

- Early-development showcase of DT: https://www.youtube.com/watch?v=6NCK1_Gycps
- Showchase of scene containing 3D reconstruction of warehouse: https://www.youtube.com/watch?v=DBkUR780ihM&t=177s
- Full Application showcase: https://www.youtube.com/watch?v=3Wb2je1Pz6w&t=55s

## Useful information for web requests in Unity.
web requests are possible on Unity only and exclusively with the class they provide us 'UnityWebRequest', other external classes or libraries could cause problems in builds.
You can find examples on how to use it here: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html

Usually the server responds with a Json, in order to convert the Json in a programming object we have to deserialize it, but before we must define the class that represents the data we are going to receive.
In order to create that class I suggest 2 ways:

  1. Copy the json response from the server (or from a sample response usually findable in the Swagger)
  2. Using this service: https://json2csharp.com/
     
And then paste the class in a C# script.
Replace the ‘RootObject’ class name with the name you want (better if equals to file name, for consistency).

Unity wants that every class is set as public and Serializable in order to be able to serialize/deserialize it, more info here: https://docs.unity3d.com/ScriptReference/Serializable.html

Only public fields are serializable/deserializable.
So, we must remove all the getters and setters, if any.

To Serialize objects to json, or to deserialize json to get objects, with Unity the class recommended is ‘JsonUtility’.
Here some instructions on how to use it: https://dev.rbcafe.com/unity/unity-5.3.3/en/Manual/JSONSerialization.html

## Useful information for Navigation Mesh in Unity.

In order to get smoother navigation in Unity, we could use the AI solution Unity provides (AI Navigation).
Here is a good video to start with. I suggest to watch it before go on: https://www.youtube.com/watch?v=u2EQtrdgfNs

Basically you have to download the AI Navigation package, create a NavMeshSurface, after setting your environment (floor, wall, and non-movable objects) as static, you can create your agent type (Its already set up in this project) and bake the surface.

I added to the Scene ‘NavMeshMovement’ to the player the script ‘NavMeshAgent’ and set everything as the MiR agent created before.

I set to the agent an acceleration of 120 in order to simulate a linear speed as much similar as possible.

A script called ‘PlayerMir_NavMesh’ is responsible for the AI navigation, which is similar to ‘PlayerMir’ but using the NavMesh for the movement and ‘Quaternion.Slerp’ for the rotation. While the player mir uses "MoveTowards" instead (wich also works, but can be tideous to tune for correct movement)

