using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowDavid : MonoBehaviour 
{
    //public variables
    public Transform david;
    public float minDistance;
    public float maxDistance;
    public float yOffset;
    public float yLerpSpeed;
    public float sensitivity;

    //private variables
    private InputManager inputManager;

    //unity methods
    private void Start() 
	{
        inputManager = ComponentManager<InputManager>.Value;
    }
    private void Update()
    {
        Vector3 dirToCamera = transform.position - david.position;

        float input = inputManager.cameraAxis.x * sensitivity * Time.deltaTime;
        Vector3 rotatedVector = Quaternion.AngleAxis(input, Vector3.up) * dirToCamera;
        dirToCamera = rotatedVector;

        float distance = dirToCamera.magnitude;

        float desiredDistance = distance;
        if (distance < minDistance)
        {
            desiredDistance =  minDistance;
        }
        else if (distance > maxDistance)
        {
            desiredDistance = maxDistance;
        }

        Vector3 targetPosition = new Vector3(0f, Mathf.Lerp(transform.position.y, david.position.y + yOffset, Time.deltaTime * yLerpSpeed), 0f);
        if (desiredDistance != distance)
        {
            targetPosition += david.position.XZOnly() + dirToCamera.XZOnly().normalized * desiredDistance;

        }
        transform.position = targetPosition;
        
        transform.LookAt(david);
    }

    //public methods

    //private methods
}
