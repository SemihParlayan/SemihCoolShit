using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour 
{
    //public variables

    //private variables
    private bool canKill = true;

	//unity methods

	//public methods
    public virtual bool Kill()
    {
        if (canKill)
        {
            canKill = false;
            Invoke("ResetCanKill", .1f);
            return true;
        }
        return false;
    }

	//private methods
    private void ResetCanKill()
    {
        canKill = true;
    }
}
