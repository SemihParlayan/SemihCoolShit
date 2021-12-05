using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour 
{
    //public variables
    public static CoroutineManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                GameObject.FindObjectOfType<CoroutineManager>();
            }
            return singleton;
        }
    }
    private static CoroutineManager singleton;

    //private variables
    private static bool initialized = false;

    //unity methods
    private void Awake()
    {
        if (initialized)
            return;
        initialized = true;
        singleton = this;
    }

    //public methods

    //private methods
}
