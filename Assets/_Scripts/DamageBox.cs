using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour 
{
    //public variables

    //private variables

    //unity methods
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.transform.CompareTag("Player"))
        {
            Entity entity = collision.transform.GetComponent<Entity>();
            entity.Kill();
        }
    }
    //public methods

    //private methods
}
