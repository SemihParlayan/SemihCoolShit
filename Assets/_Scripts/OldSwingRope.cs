using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSwingRope : MonoBehaviour 
{
    //public variables
    [SerializeField] private LineRenderer line = null;
    [SerializeField] private float gravity = -50f;
    [SerializeField] private float maxSwingMagnitude;
    [SerializeField] private float swingSpeed;
    [SerializeField] private AnimationCurve ropeLerpSpeed;

    //private variables
    private Vector3 swingPoint;
    private bool initialized = false;
    private float yVelocity = 0f;
    private float length;
    private Vector3 prevSwingMovement;

    //unity methods
    private void Update()
    {
        if (initialized)
        {
            Vector3 center = swingPoint;
            Vector3 dirToCenter = (center - transform.position).normalized;
            Vector3 start = transform.position;
            Vector3 newPosition = start + prevSwingMovement;

            Vector3 centerToNewPos = (newPosition - center).normalized;
            newPosition = center + centerToNewPos * length;

            Vector3 mov = newPosition - start;
            mov.y += gravity * Time.deltaTime;
            mov = Vector3.ClampMagnitude(mov, maxSwingMagnitude);
            transform.position += mov * Time.deltaTime * swingSpeed;

            prevSwingMovement = mov;

            float interval = length / line.positionCount;
            for (int i = 0; i < line.positionCount; i++)
            {
                line.SetPosition(i, transform.position + (dirToCenter * interval * i));
            }

            int linePosCount = line.positionCount;
            line.SetPosition(0, transform.position);
            line.SetPosition(linePosCount - 1, center);
            float totalLength = Vector3.Distance(transform.position, center);
            float indexInterval = totalLength / (linePosCount - 2);
            for (int i = 1; i < linePosCount - 1; i++)
            {
                float normalizedIndex = Mathf.Clamp01(i / (linePosCount - 2f));
                float lerpSpeed = ropeLerpSpeed.Evaluate(normalizedIndex);
                Vector3 targetPos = transform.position + dirToCenter * (indexInterval * i);
                Vector3 prevPos = line.GetPosition(i);
                Vector3 pos = Vector3.Lerp(prevPos, targetPos, Time.deltaTime * lerpSpeed);
                line.SetPosition(i, pos);
            }
        }
    }

    //public methods
    public void Initialize(LineRenderer oldLine, Vector3 swingPoint, Vector3 startPoint)
    {
        initialized = true;

        line.positionCount = oldLine.positionCount;

        this.transform.position = startPoint;
        this.swingPoint = swingPoint;
        this.length = (swingPoint - startPoint).magnitude;
    }

	//private methods
}
