using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour 
{
    //public variables
    [SerializeField] private float speed;
    [SerializeField] private float height;
    [SerializeField] private float maxOffset;

    //private variables
    private Vector3 startPos;
    private float offset;


	//unity methods
	private void Start() 
	{
        startPos = transform.position;
        offset = Random.Range(0, maxOffset);
	}
    private void Update()
    {
        transform.position = startPos + new Vector3(0f, Mathf.Sin(speed * Time.time + offset) * height, 0f);
    }


    //public methods

    //private methods
}
