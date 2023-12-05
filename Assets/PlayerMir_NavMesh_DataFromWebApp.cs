

using System.Collections;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UIElements;


[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMir_NavMesh_DataFromWebApp : MonoBehaviour
{
    NavMeshAgent MiR_navMeshAgent;
    const float Offset = 500f;

    private float pollintTime = .5f;

    private bool isMiRInitialized = false;


    private void Start()
    {
        //Cache the navMesh assigned to this gameobject
        MiR_navMeshAgent = GetComponent<NavMeshAgent>();
    }
    /// <summary>
    /// Move the MiR by position data.
    /// a NavMeshAgent is used in order to reach the destination provided.
    /// </summary>
    /// <param name="mirPositionInstance">position data</param>
    /// <returns></returns>
    public void MoveMiR_WebGL(string mirPositionDataJson)
    {
        
        if (isMiRInitialized)
        {
            MirPositionClass mirPositionInfo = JsonUtility.FromJson<MirPositionClass>(mirPositionDataJson);

            float yPosition = mirPositionInfo.status.position.x;
            float xPosition = -mirPositionInfo.status.position.y;
            float oriPosition = mirPositionInfo.status.position.orientation;
            float linSpeed = mirPositionInfo.status.velocity.linear;
            float angSpeed = mirPositionInfo.status.velocity.angular;

            MiR_navMeshAgent.speed = linSpeed + 1f;
            MiR_navMeshAgent.angularSpeed = angSpeed + 1f; //10;// angSpeed;

            // Set rotation
            Quaternion targetRotation = Quaternion.Euler(0.0f, oriPosition, 0.0f);
            StartCoroutine(RotateOverTime(this.gameObject, pollintTime, targetRotation));

            // or to rotate check which if this is more realistic
            //StartCoroutine(WaitTillNavMeshAgentHasArrivedAndThenRotate(targetRotation));

            //Go to the target
            Vector3 newPosition = new Vector3(xPosition + Offset, transform.position.y, yPosition + Offset);
            MiR_navMeshAgent.SetDestination(newPosition);

            // side note: suppose that for some reason you are not able to retrieve data for example for 4 seconds.
            // and then you get the new last data, which maybe is 10 units away from your last position and the rotation is inverted (current rotation - 180 degrees).
            // in that case, considering the current implementation, it will first start the rotation and then the movement.
            // maybe the solution is:
            // once new real time data arrives, modify the rotation with the PREVIOUS cached orientation info. 
            // or maybe rotate after arrived at target position
            // btw to make it totally realistic you should implement here an algorithm in order to know if has rotated or moved before
        }
        else
        {
            InitializeMiR(mirPositionDataJson);
            isMiRInitialized = true;
        }

    }

    public void SetPollingTime(float pollingTime)
    {
        pollintTime = pollingTime;
    }


    /// <summary>
    /// Teleport the MiR by the given position data.
    /// Should be used when starting the application, or after a pause.
    /// </summary>
    /// <param name="mirPositionInstance">position data</param>
    /// <returns></returns>
    public void InitializeMiR(string mirPositionDataJson)
    {
        MirPositionClass mirPositionInfo = JsonUtility.FromJson<MirPositionClass>(mirPositionDataJson);

        float yPosition = mirPositionInfo.status.position.x;
        float xPosition = -mirPositionInfo.status.position.y;
        float oriPosition = mirPositionInfo.status.position.orientation;
        float linSpeed = mirPositionInfo.status.velocity.linear;
        float angSpeed = mirPositionInfo.status.velocity.angular;

        MiR_navMeshAgent.speed = linSpeed + 1f;
        MiR_navMeshAgent.angularSpeed = angSpeed + 1f; //10;// angSpeed;

        //Set rotation
        Quaternion lookRotation = Quaternion.Euler(0.0f, oriPosition, 0.0f);
        StartCoroutine(RotateOverTime(this.gameObject, .5f, lookRotation));

        //Teleport to target, fixing the agent on the navmesh
        Vector3 newPosition = new Vector3(xPosition + Offset, transform.position.y, yPosition + Offset);
        MiR_navMeshAgent.Warp(newPosition);
    }

    private IEnumerator RotateOverTime(GameObject targetObject, float transitionDuration, Quaternion target)
    {
        float counter = 0f;

        while (counter < transitionDuration)
        {
            counter += Time.deltaTime;
            targetObject.transform.rotation = Quaternion.Slerp(targetObject.transform.rotation, target, counter / transitionDuration);
            yield return null;
        }
    }

}
