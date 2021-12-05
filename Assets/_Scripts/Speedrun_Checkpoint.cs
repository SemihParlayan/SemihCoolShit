using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedrun_Checkpoint: MonoBehaviour 
{
    //properties
    public bool IsActivated => isActivated;

    //public variables
    [SerializeField] private ParticleSystem particles = null;
    [SerializeField] private GameObject insideObject = null;

    //private variables
    private bool isActivated = false;

    //unity methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            ComponentManager<SpeedrunManager>.Value.OnCheckpointReached(this);
        }
    }

    //public methods
    public void ResetCheckpoint()
    {
        isActivated = false;
        insideObject?.SetActiveSafe(true);
        particles?.Stop();
    }

    //private methods
    public void ActivateCheckpoint()
    {
        isActivated = true;
        insideObject?.SetActiveSafe(false);
        particles?.Play();
    }
}
