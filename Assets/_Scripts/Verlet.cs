using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Verlet : MonoBehaviour
{
    //public variables
    public int constrictCount = 50;
    public float gravity = -1f;
    public List<Point> points;
    public List<Stick> sticks;

    //private variables


    //unity methods
    private void Awake()
    {
        ComponentManager<Verlet>.Value = this;
    }

    private void FixedUpdate()
    {
        Simulate();

        for (int i = 0; i < constrictCount; i++)
        {
            Constrict();
        }
    }
    private void OnDrawGizmos()
    {
        if (points == null)
            return;

        float radius = 0.1f;
        foreach (Point point in points)
        {
            Gizmos.DrawSphere(point.position, radius);
        }
        foreach(Stick stick in sticks)
        {
            Gizmos.DrawLine(stick.pointA.position, stick.pointB.position);
        }
    }

    //public methods
    public Point CreatePoint(bool locked, Vector3 position, Vector3 normal, Transform anchorTransform = null)
    {
        Point point = new Point();
        point.position = position;
        point.prevPosition = position;
        point.normal = normal;
        point.transform = anchorTransform;

        //if (prefabModel)
        //{
        //    Transform model = Instantiate(prefabModel, position, Quaternion.identity) as Transform;
        //    point.model = model;
        //    point.model.up = normal;
        //}
        return point;
    }
    public ChainData CreateChain(Vector3 pointA, Vector3 pointB, float pointInterval, Vector3 normal, bool lockA, bool lockB, Transform prefabModel = null)
    {
        ChainData data = new ChainData();

        float totalDst = Vector3.Distance(pointA, pointB);
        int numberOfPoints = Mathf.Clamp(Mathf.CeilToInt(totalDst / pointInterval), 1, int.MaxValue);
        float pointDistance = totalDst / numberOfPoints;
        Vector3 dir = (pointB - pointA).normalized;

        Point prevPoint = null;
        for (int i = 0; i < numberOfPoints; i++)
        {
            Point point = CreatePoint(false, pointA + (dir * pointDistance) * i, normal, prefabModel);
            if (i == 0)
            {
                point.isLocked = lockA;
                //point.transform = anchorA;
                data.pointA = point;
            }
            else if (i == numberOfPoints - 1)
            {
                point.isLocked = lockB;
                //point.transform = anchorB;
                data.pointB = point;
            }
            points.Add(point);
            data.points.Add(point);

            if (prevPoint != null)
            {
                Stick newStick = new Stick(prevPoint, point);
                sticks.Add(newStick);
                data.sticks.Add(newStick);
            }

            prevPoint = point;
        }

        return data;
    }
    public void DeleteChainData(ChainData data)
    {
        for (int i = 0; i < data.points.Count; i++)
        {
            points.Remove(data.points[i]);
        }

        for (int i = 0; i < data.sticks.Count; i++)
        {
            sticks.Remove(data.sticks[i]);
        }
    }


    //private methods
    private void Simulate()
    {
        if (points == null)
            return;

        foreach (Point point in points)
        {
            if (point.isLocked)
            {
                if (point.transform != null)
                {
                    point.position = point.transform.position;
                    point.normal = point.transform.up;
                    point.prevPosition = point.position;
                }

                continue;
            }

            Vector3 posBeforeUpdate = point.position;
            point.position += (point.position - point.prevPosition);
            point.position += Vector3.up * gravity * Time.deltaTime;
            point.prevPosition = posBeforeUpdate;

            if (point.transform != null)
            {
                point.transform.position = point.position;
                point.transform.up = point.normal;
            }
        }
    }
    private void Constrict()
    {
        if (sticks == null)
            return;

        foreach (Stick stick in sticks)
        {
            Vector3 stickCentre = (stick.pointA.position + stick.pointB.position) / 2;
            Vector3 stickDir = (stick.pointA.position - stick.pointB.position).normalized;

            if (!stick.pointA.isLocked)
            {
                stick.pointA.position = stickCentre + stickDir * stick.length / 2;
                if (stick.pointA.position.y < 0)
                    stick.pointA.position.y = 0;
            }
            if (!stick.pointB.isLocked)
            {
                stick.pointB.position = stickCentre - stickDir * stick.length / 2;
                if (stick.pointB.position.y < 0)
                    stick.pointB.position.y = 0;
            }
        }
    }
}
public class ChainData
{
    public Point pointA = null;
    public Point pointB = null;
    public List<Point> points = new List<Point>();
    public List<Stick> sticks = new List<Stick>();
}
[System.Serializable]
public class Point
{
    public bool isLocked;
	public Vector3 position;
	[HideInInspector] public Vector3 prevPosition;
	[HideInInspector] public Vector3 normal;
    public Transform transform;
}
[System.Serializable]
public class Stick
{
	public float length;
	public Point pointA;
	public Point pointB;

    public Stick(Point pointA, Point pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.length = Vector3.Distance(pointA.position, pointB.position);
    }
}
