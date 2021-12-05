using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRope : MonoBehaviour 
{
	//public variables
	[SerializeField] private LineRenderer line;
	[SerializeField] private float pointInterval = 0.2f;
	[SerializeField] private float lifeTime = 5f;

    //private variables
    private static Verlet verlet;
    private ChainData chainData;

	//unity methods
	public void Initialize(Vector3 start, Vector3 end)
	{
        if (verlet == null)
            verlet = ComponentManager<Verlet>.Value;

        chainData = verlet.CreateChain(start, end, pointInterval, Vector3.up, true, false);
        line.positionCount = chainData.points.Count;

        Destroy(gameObject, lifeTime);
    }
    private void Update()
    {
        for (int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, chainData.points[i].position);
        }
    }
    private void OnDestroy()
    {
        if (chainData != null)
        {
            if (verlet == null)
                verlet = ComponentManager<Verlet>.Value;
            verlet.DeleteChainData(chainData);
        }
    }

    //public methods

    //private methods
}
