using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRawrDavid : MonoBehaviour 
{
    //public variables
    public Transform target;
    public Transform camera;
    public float speed = 50f;

    //private variables


    //unity methods
    private void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        Vector3 dir = camera.transform.position - target.position;
        float dist = dir.magnitude;
        dir.Normalize();

        float degree = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        degree += input.x * speed * Time.deltaTime;

        Vector3 newPos = new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), 0f, Mathf.Sin(degree * Mathf.Deg2Rad));
        camera.transform.position += (newPos - camera.transform.position);
        Debug.Log(degree);
    }

    //public methods

    //private methods
}
