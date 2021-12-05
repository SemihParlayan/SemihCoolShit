using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingPoint : MonoBehaviour 
{
    public static List<SwingPoint> AllSwingPoints = new List<SwingPoint>();

    //public variables


    //private variables


    //unity methods
    private void OnEnable()
    {
        AllSwingPoints.Add(this);
    }
    private void OnDisable()
    {
        AllSwingPoints.Remove(this);
    }

    //public methods
    public static SwingPoint GetClosestPoint(Vector3 worldPoint, float maxDistance)
    {
        SwingPoint closest = null;
        float minDst = float.MaxValue;
        foreach (SwingPoint point in AllSwingPoints)
        {
            float dst = Vector3.Distance(point.transform.position, worldPoint);
            if (dst < minDst && dst <= maxDistance)
            {
                minDst = dst;
                closest = point;
            }
        }

        return closest;
    }

    //private methods
}
