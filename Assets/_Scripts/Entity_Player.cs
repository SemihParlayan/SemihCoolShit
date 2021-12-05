using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Player : Entity 
{
    //public variables
    [SerializeField] private GameObject ragdoll;
    [SerializeField] private Transform respawnPoint;

    //private variables

    //unity methods
    public override bool Kill()
    {
        if (base.Kill())
        {
            StartCoroutine(SpawnRagdoll(transform.position, transform.rotation));

            transform.position = respawnPoint.position;
            transform.GetComponent<MoveCharacter>().ChangeMoveState(MoveCharacter.MoveState.Moving);
            return true;
        }
        return false;
    }

    //public methods

    //private methods
    private IEnumerator SpawnRagdoll(Vector3 pos, Quaternion rot)
    {
        yield return new WaitForSeconds(.1f);
        GameObject rag = GameObject.Instantiate(ragdoll, pos, rot) as GameObject;
        Destroy(rag.gameObject, 60f);
    }
}
