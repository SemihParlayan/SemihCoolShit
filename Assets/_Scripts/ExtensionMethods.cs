using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class ExtensionMethods
{
    //Int
    public static void IncrementCycle(ref this int number, int increment, int min, int max)
    {
        number += increment;
        if (number > max)
            number = min;
        if (number < min)
            number = max;
    }

    //Vector3
    public static Vector3 XZOnly(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
    public static void Clamp(this Vector3 vector, float min, float max)
    {
        vector.x = Mathf.Clamp(vector.x, min, max);
        vector.y = Mathf.Clamp(vector.y, min, max);
        vector.z = Mathf.Clamp(vector.z, min, max);
    }
    public static float DistanceXZ(this Vector3 vector, Vector3 other)
    {
        vector.y = other.y = 0;

        return Vector3.Distance(vector, other);
    }
    public static bool IsLessThan(this Vector3 vector, Vector3 other)
    {
        return vector.x < other.x && vector.y < other.y;
    }
    public static bool IsLessThanOrEqual(this Vector3 vector, Vector3 other)
    {
        return vector.x <= other.x && vector.y <= other.y;
    }
    public static bool IsGreaterThan(this Vector3 vector, Vector3 other)
    {
        return vector.x > other.x && vector.y > other.y;
    }
    public static bool IsGreaterThanOrEqual(this Vector3 vector, Vector3 other)
    {
        return vector.x >= other.x && vector.y >= other.y;
    }
    public static Vector3 RoundToDividable(this Vector3 vector, float dividableAmount)
    {
        Vector3 roundedVector = vector;
        roundedVector.x = Mathf.RoundToInt(roundedVector.x / dividableAmount) * dividableAmount;
        roundedVector.y = Mathf.RoundToInt(roundedVector.y / dividableAmount) * dividableAmount;
        roundedVector.z = Mathf.RoundToInt(roundedVector.z / dividableAmount) * dividableAmount;
        return roundedVector;
    }

    //Vector2 
    public static bool IsLessThan(this Vector2 vector, Vector2 other)
    {
        return vector.x < other.x && vector.y < other.y;
    }
    public static bool IsLessThanOrEqual(this Vector2 vector, Vector2 other)
    {
        return vector.x <= other.x && vector.y <= other.y;
    }
    public static bool IsGreaterThan(this Vector2 vector, Vector2 other)
    {
        return vector.x > other.x && vector.y > other.y;
    }
    public static bool IsGreaterThanOrEqual(this Vector2 vector, Vector2 other)
    {
        return vector.x >= other.x && vector.y >= other.y;
    }

    //Float
    public static float BiggestOfValues(params float[] values)
    {
        float biggest = values[0];
        for (int i = 1; i < values.Length; i++)
        {
            if (values[i] > biggest)
                biggest = values[i];
        }
        return biggest;
    }
    public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float normal = Mathf.InverseLerp(fromMin, fromMax, value);
        return Mathf.Lerp(toMin, toMax, normal);
        //return (value - fromMin) / (fromMax - fromMin) * (toMax - fromMax) + fromMax;
    }

    //String
    public static bool IsNullOrEmpty(this string text)
    {
        return string.IsNullOrEmpty(text);
    }
    public static bool IsValid(this string text)
    {
        return !text.IsNullOrEmpty();
    }
    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        if (source == null) return false;
        return source.IndexOf(toCheck, comp) >= 0;
    }
    public static bool ContainsInvalidWindowsCharacter(this string source)
    {
        /*
                / \ : * ? " < > |
        */
        if (source == null) return false;

        return
            source.Contains("/") ||
            source.Contains(@"\") ||
            source.Contains(":") ||
            source.Contains("*") ||
            source.Contains("?") ||
            source.Contains("\"") ||
            source.Contains("<") ||
            source.Contains(">") ||
            source.Contains("|");
    }

    //Actions
    public static void SubscribeMethod(this System.Action action, System.Action method)
    {
        action += method;
    }
    public static void CallSafe(this System.Action action)
    {
        if (action != null)
            action();
    }
    public static void CallSafe<T1>(this System.Action<T1> action, T1 arg1)
    {
        if (action != null)
            action(arg1);
    }
    public static void CallSafe<T1, T2>(this System.Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        if (action != null)
            action(arg1, arg2);
    }
    public static void CallSafe<T1, T2, T3>(this System.Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        if (action != null)
            action(arg1, arg2, arg3);
    }
    public static void CallSafe<T1, T2, T3, T4>(this System.Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (action != null)
            action(arg1, arg2, arg3, arg4);
    }


    //Keycode
    public static string ToShortString(this KeyCode keycode)
    {
        switch (keycode)
        {
            case KeyCode.None:
                return "Not set";
            case KeyCode.Mouse0:
                return "LMB";
            case KeyCode.Mouse1:
                return "RMB";
            case KeyCode.Mouse2:
                return "MMB";
            case KeyCode.Escape:
                return "Esc";
            case KeyCode.CapsLock:
                return "Caps";
            case KeyCode.Comma:
                return ",";
            case KeyCode.Period:
                return ".";
            case KeyCode.DownArrow:
                return "DArrow";
            case KeyCode.LeftArrow:
                return "LArrow";
            case KeyCode.RightArrow:
                return "RArrow";
            case KeyCode.UpArrow:
                return "UArrow";
            case KeyCode.LeftShift:
                return "LShift";
            case KeyCode.RightShift:
                return "RShift";
            case KeyCode.LeftControl:
                return "LCtrl";
            case KeyCode.RightControl:
                return "RCtrl";
            case KeyCode.LeftAlt:
                return "LAlt";
            case KeyCode.RightAlt:
                return "RAlt";
            case KeyCode.Alpha1:
                return "1";
            case KeyCode.Alpha2:
                return "2";
            case KeyCode.Alpha3:
                return "3";
            case KeyCode.Alpha4:
                return "4";
            case KeyCode.Alpha5:
                return "5";
            case KeyCode.Alpha6:
                return "6";
            case KeyCode.Alpha7:
                return "7";
            case KeyCode.Alpha8:
                return "8";
            case KeyCode.Alpha9:
                return "9";
            case KeyCode.Alpha0:
                return "0";
            case KeyCode.Keypad0:
                return "Num0";
            case KeyCode.Keypad1:
                return "Num1";
            case KeyCode.Keypad2:
                return "Num2";
            case KeyCode.Keypad3:
                return "Num3";
            case KeyCode.Keypad4:
                return "Num4";
            case KeyCode.Keypad5:
                return "Num5";
            case KeyCode.Keypad6:
                return "Num6";
            case KeyCode.Keypad7:
                return "Num7";
            case KeyCode.Keypad8:
                return "Num8";
            case KeyCode.Keypad9:
                return "Num9";
        }
        return keycode.ToString();
    }

    //Array
    public static bool ContainsItems(this Array array)
    {
        return array != null && array.Length > 0;
    }
    public static bool EqualsOtherArray<T>(this T[] array, T[] other)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (!array[i].Equals(other[i]))
            {
                return false;
            }
        }
        return true;
    }

    //List
    public static bool ContainsItems<T>(this List<T> array)
    {
        return array != null && array.Count > 0;
    }
    public static void AddRangeUniqueOnly<T>(this List<T> array, IEnumerable<T> range)
    {
        IEnumerator en = range.GetEnumerator();

        while (en.MoveNext())
        {
            if (!array.Contains((T)(en.Current)))
            {
                array.Add((T)(en.Current));
            }
        }
    }
    public static bool AddUniqueOnly<T>(this List<T> array, T itemToAdd)
    {
        if (itemToAdd != null && !array.Contains(itemToAdd))
        {
            array.Add(itemToAdd);
            return true;
        }
        return false;
    }

    //Dictionary
    public static bool ContainsItems<T, B>(this Dictionary<T, B> dic)
    {
        return dic != null && dic.Count > 0;
    }

    //Behaviour
    public static bool IsNotNull(this Behaviour behaviour)
    {
        return behaviour != null;
    }

    //Quaternion
    public static Quaternion Random(this Quaternion quaternion)
    {
        return Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)));
    }

    //GameObject
    public static bool SetActiveSafe(this GameObject gameObject, bool value)
    {
        if (gameObject == null)
        {
            Debug.Log("gameobject was null tried to set activate value to" + value.ToString());
            return false;
        }
        if (gameObject.activeSelf != value)
        {
            gameObject.SetActive(value);
            return true;
        }
        return false;
    }
    public static void SetLayerSafe(this GameObject gameObject, int layer)
    {
        if (!gameObject.layer.Equals(layer))
        {
            gameObject.layer = layer;
        }
    }
    public static void SetLayerRecursivly(this GameObject parentGameObject, int layer)
    {
        for (int i = 0; i < parentGameObject.transform.childCount; i++)
        {
            Transform child = parentGameObject.transform.GetChild(i);
            child.gameObject.SetLayerSafe(layer);
            SetLayerRecursivly(child.gameObject, layer);
        }
    }

    //Transform
    public static bool SetParentSafe(this Transform transform, Transform newParent)
    {
        if (transform.parent != newParent)
        {
            transform.SetParent(newParent);
            return true;
        }

        return false;
    }
    public static Transform FindChildRecursively(this Transform tranform, string childName)
    {
        Transform foundChild = tranform.Find(childName);
        if (foundChild != null)
            return foundChild;

        foreach (Transform child in tranform)
        {
            foundChild = child.FindChildRecursively(childName);
            if (foundChild != null)
                return foundChild;
        }
        return null;
    }
    public static void GetComponentsInChildrenRecursively<T>(this Transform tranform, ref List<T> result)
    {
        if (result == null)
            result = new List<T>();

        foreach (Transform child in tranform)
        {
            T t = child.GetComponent<T>();
            if (t != null)
            {
                result.AddUniqueOnly(t);
            }

            child.GetComponentsInChildrenRecursively(ref result);
        }
    }
    public static void FindChildrenByTag(this Transform parent, ref List<Transform> children, string tag)
    {
        Transform parentTransform = parent.transform;
        foreach (Transform child in parentTransform)
        {
            if (child.tag == tag)
            {
                if (children == null)
                    children = new List<Transform>();
                children.Add(child);
            }
            FindChildrenByTag(child, ref children, tag);
        }
    }
    public static void FindChildrenByLayer(this Transform parent, ref List<Transform> children, int layer)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.gameObject.layer.Equals(layer))
            {
                children.Add(child);
            }
            FindChildrenByLayer(child, ref children, layer);
        }
    }

#if UNITY_EDITOR
    [MenuItem("CONTEXT/Transform/SortChildren", false, 150)]
    public static void CopyTransform()
    {
        Transform parent = Selection.activeTransform;
        if (parent != null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child != null)
                {
                    child.localPosition = new Vector3(i * 10f, 0f, 0f);
                }
            }
        }
    }
#endif

    //Animator
    public static void SaveAnimatorParameters(this Animator anim, ref AnimatorStorage storage)
    {
        if (anim.parameterCount == 0)
            return;

        if (storage == null)
        {
            storage = new AnimatorStorage((uint)anim.parameterCount);
        }

        for (int i = 0; i < anim.parameterCount; i++)
        {
            AnimatorControllerParameter param = anim.parameters[i];

            if (storage.storedParamaters != null)
                storage.storedParamaters[i].SaveParameter(param, anim);
        }
    }
    public static void LoadAnimatorParameters(this Animator anim, AnimatorStorage storage)
    {
        if (storage == null)
            return;

        if (storage.storedParamaters == null)
            return;

        for (int i = 0; i < storage.storedParamaters.Length; i++)
        {
            storage.storedParamaters[i].LoadParameter(anim);
        }
    }

    //Behaviour
    public static void SetEnabledSafe(this Behaviour behaviour, bool value)
    {
        if (behaviour.enabled != value)
            behaviour.enabled = value;
    }

    //Particle systems
    public static void PlaySafe(this ParticleSystem particleSystem)
    {
        if (!particleSystem.isPlaying)
            particleSystem.Play();
    }
    public static void StopSafe(this ParticleSystem particleSystem)
    {
        if (particleSystem.isPlaying)
            particleSystem.Stop();
    }
    public static void PauseSafe(this ParticleSystem particleSystem)
    {
        if (!particleSystem.isPaused)
            particleSystem.Pause();
    }

    //Navmesh Agent
    public static void StopSafe(this NavMeshAgent agent)
    {
        if (agent.isOnNavMesh && agent.enabled && !agent.isStopped)
            agent.isStopped = true;
    }
    public static void ResumeSafe(this NavMeshAgent agent)
    {
        if (agent.isOnNavMesh && agent.enabled && agent.isStopped)
            agent.isStopped = false;
    }
    public static void TurnOffSafe(this NavMeshAgent agent)
    {
        if (agent.enabled)
        {
            if (agent.isOnNavMesh && !agent.isStopped)
                agent.isStopped = true;
            agent.enabled = false;
        }
    }
    public static void TurnOnSafe(this NavMeshAgent agent)
    {
        if (!agent.enabled)
        {
            //if (agent.isOnNavMesh == false)
            //{
            //    agent.transform.position = Helper.GetClosestPointOnNavMesh(agent.transform.position);
            //}
            agent.enabled = true;

            if (agent.isOnNavMesh && agent.isStopped)
                agent.isStopped = false;
        }
    }

    //Color
    public static Color RandomColor(this Color color)
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
    }
    
    //Raycashit
    public static bool IsValid(this RaycastHit hit)
    {
        return hit.transform != null;
    }
}

//Animator
public class AnimatorStorage
{
    public AnimatorParameterStorage[] storedParamaters;

    public AnimatorStorage(uint parameterCount)
    {
        if (parameterCount > 0)
        {
            storedParamaters = new AnimatorParameterStorage[parameterCount];
        }
    }
}
public struct AnimatorParameterStorage
{
    public AnimatorControllerParameterType type;
    public string paramaterName;
    public bool boolValue;
    public float floatValue;
    public int intValue;

    public void SaveParameter(AnimatorControllerParameter param, Animator anim)
    {
        this.paramaterName = param.name;
        this.type = param.type;

        switch (param.type)
        {
            case AnimatorControllerParameterType.Float:
                floatValue = anim.GetFloat(param.name);
                break;
            case AnimatorControllerParameterType.Int:
                intValue = anim.GetInteger(param.name);
                break;
            case AnimatorControllerParameterType.Bool:
                boolValue = anim.GetBool(param.name);
                break;
        }
    }
    public void LoadParameter(Animator anim)
    {
        switch (type)
        {
            case AnimatorControllerParameterType.Float:
                anim.SetFloat(paramaterName, floatValue);
                break;
            case AnimatorControllerParameterType.Int:
                anim.SetInteger(paramaterName, intValue);
                break;
            case AnimatorControllerParameterType.Bool:
                anim.SetBool(paramaterName, boolValue);
                break;
        }
    }
}