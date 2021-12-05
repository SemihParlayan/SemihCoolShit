using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncepad : MonoBehaviour 
{
    //public variables
    [SerializeField] private Animator animator;

	//private variables


	//unity methods
    public void PlayBounceEffect()
    {
        animator.SetTrigger("Bounce");
    }

	//public methods

	//private methods
}
