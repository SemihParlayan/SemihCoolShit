using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour 
{
    //public variables
    [SerializeField] private NavMeshAgent agent;

    //private variables
    private static MoveCharacter player;

    //unity methods
    private void Start()
    {
        player = ComponentManager<MoveCharacter>.Value;
    }
    private void OnDrawGizmos()
    {
        Vector3 samplePos = new Vector3(transform.position.x, 0f, transform.position.z);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(samplePos, out hit, 25f, NavMesh.AllAreas))
        {
            Gizmos.DrawCube(hit.position, Vector3.one * 0.5f);
        }
    }
    private void Update()
    {
        Vector3 samplePos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(samplePos, out hit, 25f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    //public methods

    //private methods
}
