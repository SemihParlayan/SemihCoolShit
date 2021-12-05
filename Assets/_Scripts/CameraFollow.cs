using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    //public variables
    public Transform cameraParent;
    public Transform target;
    public float moveSpeed;
    public float rotateSpeed;
    public float sensitivity;

    //private variables
    private InputManager inputManager;
    private float currentRotation;
    private float targetRotation;

    //unity methods
    private void Start()
    {
        inputManager = ComponentManager<InputManager>.Value;
        currentRotation = targetRotation = cameraParent.eulerAngles.y;
    }
    private void Update()
    {
        cameraParent.position = Vector3.Lerp(cameraParent.position, target.position, Time.deltaTime * moveSpeed);

        targetRotation += inputManager.cameraAxis.x * sensitivity * Time.deltaTime;
        if (targetRotation > 360)
            targetRotation = targetRotation - 360;
        else if (targetRotation < 0)
            targetRotation += 360;

        currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, Time.deltaTime * rotateSpeed);
        cameraParent.eulerAngles = new Vector3(0f, currentRotation, 0f);
    }

    //public methods

    //private methods
}
