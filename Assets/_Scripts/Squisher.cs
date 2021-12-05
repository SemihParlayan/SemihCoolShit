using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squisher : MonoBehaviour 
{
    //public variables

    //private variables
    private Coroutine squishRoutine;


	//unity methods
    public Coroutine Squish(Transform transform, SO_SquisherSettings setting)
    {
        if (squishRoutine == null)
        {
            squishRoutine = CoroutineManager.Singleton.StartCoroutine(SquishCoroutine(transform, setting));
        }
        return squishRoutine;
    }

	//public methods

	//private methods
    private IEnumerator SquishCoroutine(Transform transform, SO_SquisherSettings setting)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(startScale.x * setting.scaleModifier.x, startScale.y * setting.scaleModifier.y, startScale.z * setting.scaleModifier.z);

        float startTime = Time.time;
        float normalizedTime = 0f;

        while(normalizedTime <= 1f)
        {
            normalizedTime = (Time.time - startTime) / (setting.time / 2f);
            transform.localScale = Vector3.Lerp(startScale, targetScale, setting.squashCurve.Evaluate(normalizedTime));
            yield return null;
        }

        startTime = Time.time;
        normalizedTime = 0f;
        while (normalizedTime <= 1f)
        {
            normalizedTime = (Time.time - startTime) / (setting.time / 2f);
            transform.localScale = Vector3.Lerp(targetScale, startScale, setting.stretchCurve.Evaluate(normalizedTime));
            yield return null;
        }

        transform.localScale = startScale;
        squishRoutine = null;
    }
}
