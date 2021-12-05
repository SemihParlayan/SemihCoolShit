using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProcedualAnimation : MonoBehaviour 
{
    //properties
    public bool IsMoving => moveRoutine != null;

    //public variables
    public ProcedualAnimation dependency;
    public Transform moveTransform;
    public Transform target;
    public float maxDistance;
    public float moveTime;
    public float moveHeight;

    private Coroutine moveRoutine;

    //private variables

    //unity methods
    private void OnDisable()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
    }
    public void Update()
    {
        float dst = Vector3.Distance(moveTransform.position, target.position);

        if (dst >= maxDistance && !IsMoving)
        {
            if (dependency != null && !dependency.IsMoving)
                moveRoutine = StartCoroutine(Move());
        }
    }

    //public methods

    //private methods
    private IEnumerator Move()
    {
        float startTime = Time.time;
        float normalizedTime = 0f;
        Vector3 start = moveTransform.position;
        while (normalizedTime <= 1f)
        {
            normalizedTime = (Time.time - startTime) / moveTime;

            moveTransform.position = Helper.GetBezierCurveValue(start, target.position, moveHeight, normalizedTime);
            yield return null;
        }
        moveTransform.position = target.position;
        moveRoutine = null;
        yield break;
    }
}
