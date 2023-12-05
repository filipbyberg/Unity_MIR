using System.Collections;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UIElements;


[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMir_NavMesh : MonoBehaviour
{
    NavMeshAgent MiR_navMeshAgent;

    const string mirDataEndpoint = "http://jiwa-api.azurewebsites.net/api/v1/mir/mir_position";

    //Create a global variable for the coroutine, so it could be stopped when needed.
    Coroutine pollingCoroutine = null;

    [Tooltip("Time to wait before sending a new web request while polling")]
    [Range(0.2f, 10f)]
    [SerializeField]
    float pollintTime = 1f;

    const float Offset = 500f;


    // Update is called once per frame
    IEnumerator Start()
    {
        //Cache the navMesh assigned to this gameobject
        MiR_navMeshAgent = GetComponent<NavMeshAgent>();

        yield return StartCoroutine(GetRequest<MirPositionClass>(mirDataEndpoint, (mirInfo) =>
            InitializeMiR(mirInfo)
            )
        );

        // Polling coroutine
        // Here we do not just call a normal function for polling data, because it would freeze the application and/or slow it down
        // we use Coroutines because they allow us to execute code across multiple frames.
        // Coroutines are useful for long iterations or polling, but also when we need to wait for next frame or an amount of time.
        
        pollingCoroutine = StartCoroutine(MonitorMirDataThenMove2());
        //pollingCoroutine = StartCoroutine(MonitorSampledMirDataThenMove(pollintTime));

        // it can be stopped using these lines of code:
        // 
        //StopAllCoroutines();
        //or
        //StopCoroutine(pollingCoroutine);
    }

    /// <summary>
    /// Test sampled data
    /// </summary>
    /// <returns></returns>

    /// <summary>
    /// Monitoring of MiR position data by polling and moving it in the application
    /// </summary>
    /// <param name="pollingInterval">time to wait before sending a new web request</param>
    /// <returns></returns>
    IEnumerator MonitorMirDataThenMove2(float pollingInterval = 1f)
    {
        WaitForSeconds pollingIntervalWait = new WaitForSeconds(pollingInterval);
        while (true)
        {
            // Wait for the interval 
            yield return pollingIntervalWait;
            yield return StartCoroutine(GetRequest<MirPositionClass>(mirDataEndpoint,
                (mirData) => MoveMiR(mirData)
            ));
        }
    }

    IEnumerator WaitTillNavMeshAgentHasArrivedAndThenRotate(Quaternion targetRotation)
    {
        while (Mathf.Abs(Vector3.Distance(MiR_navMeshAgent.destination, transform.position)) < 0.015f)
        {
            yield return null;
        }

        StartCoroutine(RotateOverTime(this.gameObject, pollintTime, targetRotation));
    }


    /// <summary>
    /// Move the MiR by position data.
    /// a NavMeshAgent is used in order to reach the destination provided.
    /// </summary>
    /// <param name="mirPositionInstance">position data</param>
    /// <returns></returns>
    private void MoveMiR(MirPositionClass mirPositionInstance)
    {

        float yPosition = mirPositionInstance.status.position.x;
        float xPosition = -mirPositionInstance.status.position.y;
        float oriPosition = -mirPositionInstance.status.position.orientation;
        float linSpeed = mirPositionInstance.status.velocity.linear;
        float angSpeed = mirPositionInstance.status.velocity.angular;

        MiR_navMeshAgent.speed = linSpeed + 0.1f;
        MiR_navMeshAgent.angularSpeed =  angSpeed + 0; //10;// angSpeed;

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


    /// <summary>
    /// Teleport the MiR by the given position data.
    /// Should be used when starting the application, or after a pause.
    /// </summary>
    /// <param name="mirPositionInstance">position data</param>
    /// <returns></returns>
    private void InitializeMiR(MirPositionClass mirPositionInstance)
    {

        float yPosition = mirPositionInstance.status.position.x;
        float xPosition = -mirPositionInstance.status.position.y;
        float oriPosition = -mirPositionInstance.status.position.orientation;
        float linSpeed = mirPositionInstance.status.velocity.linear;
        float angSpeed = mirPositionInstance.status.velocity.angular;

        Vector3 newRotation = new Vector3(0f, oriPosition, 0f);

        MiR_navMeshAgent.speed = linSpeed + 1f;
        MiR_navMeshAgent.angularSpeed = angSpeed; //10;// angSpeed;

        //Set rotation
        //Quaternion lookRotation = Quaternion.Euler(0.0f, oriPosition, 0.0f);
        //StartCoroutine(RotateOverTime(this.gameObject, .5f, lookRotation));
        transform.eulerAngles = newRotation;

        //Teleport to target, fixing the agent on the navmesh
        Vector3 newPosition = new Vector3(xPosition + Offset, transform.position.y, yPosition + Offset);
        MiR_navMeshAgent.Warp(newPosition);
    }

    private IEnumerator RotateOverTime(GameObject targetObject, float transitionDuration, Quaternion target)
    {
        float counter = 0f;

        while (counter < transitionDuration)
        {
            if (MiR_navMeshAgent.angularSpeed <= 1 && MiR_navMeshAgent.angularSpeed >= -1)
            {
                Debug.Log(MiR_navMeshAgent.angularSpeed);
                counter += Time.deltaTime;
                targetObject.transform.rotation = Quaternion.Slerp(targetObject.transform.rotation, target, (MiR_navMeshAgent.angularSpeed*0.5f) + 0.01f);
                yield return null;
            }
            else if (MiR_navMeshAgent.angularSpeed > 1)
            {
                Debug.Log(MiR_navMeshAgent.angularSpeed);
                counter += Time.deltaTime;
                targetObject.transform.rotation = Quaternion.Slerp(targetObject.transform.rotation, target, 0.05f);
                yield return null;
            }
            else
            {
                Debug.Log(MiR_navMeshAgent.angularSpeed);
                counter += Time.deltaTime;
                targetObject.transform.rotation = Quaternion.Slerp(targetObject.transform.rotation, target, -0.05f);
                yield return null;
            }

        }
    }

    public static bool isRotationApproximate(Quaternion q1, Quaternion q2, float precision)
    {
        return Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;
    }


    IEnumerator GetRequest<T>(string url, System.Action<T> callbackOnFinish)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
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
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    T mirInfo = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callbackOnFinish(mirInfo);
                    break;
            }
        }
    }
}
