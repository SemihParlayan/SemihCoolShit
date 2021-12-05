using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    //public variables
    public static System.Action OnPickupAmountChanged;
    public static int pickupAmount;

    //private variables
    private CanvasManager canvasManager;

    //unity methods
    private void Awake()
    {
        ComponentManager<GameManager>.Value = this;
    }
    private void Start()
    {
        canvasManager = ComponentManager<CanvasManager>.Value;
    }

    //public methods
    public void AddPickupAmount(int amount)
    {
        pickupAmount += amount;
        OnPickupAmountChanged?.Invoke();
    }

    //private methods
}
