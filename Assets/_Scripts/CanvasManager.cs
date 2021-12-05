using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour 
{
    //public variables
    public RectTransform aimSprite;
    public CanvasGroup pickupGroup;
    public TextMeshProUGUI pickupAmount;
    public TextMeshProUGUI speedrunTimer;

    //private variables
    private const float cameraMultiplier = 4f;


    //unity methods
    private void Awake()
    {
        ComponentManager<CanvasManager>.Value = this;

        //Pickup
        GameManager.OnPickupAmountChanged += OnPickupAmountChanged;
        pickupGroup.alpha = 0f;
    }

    //event methods
    private void OnPickupAmountChanged()
    {
        StartCoroutine(AddPickupAmountRoutine());
    }

    //public methods
    public static Vector3 WorldToScreenPoint(Vector3 worldPoint)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        return screenPoint * cameraMultiplier;
    }
    public IEnumerator AddPickupAmountRoutine()
    {
        pickupGroup.alpha = 1f;
        pickupAmount.text = GameManager.pickupAmount.ToString();

        float startTime = Time.time;
        while(Time.time - startTime < 5f)
        {
            yield return null;
        }

        while(pickupGroup.alpha > 0f)
        {
            pickupGroup.alpha = Mathf.MoveTowards(pickupGroup.alpha, 0f, Time.deltaTime);
            yield return null;
        }
    }
    //private methods
}
