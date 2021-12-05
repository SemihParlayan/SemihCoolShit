using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentManager<T> : MonoBehaviour
{
    public static T Value { get { return component; } set { component = value; } }

    //private variables
    private static T component;
}