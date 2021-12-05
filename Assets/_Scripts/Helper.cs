using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Analytics;
using System;
using System.Reflection;

public class Helper : MonoBehaviour
{
    //properties
    public static Camera MainCamera { get { if (mainCamera == null) mainCamera = Camera.main; return mainCamera; } }
    private static Camera mainCamera;

    //public variables

    //private variables
    private static CursorLockMode lockMode;

    //unity method
    static void Update()
    {
        Cursor.lockState = lockMode;
    }

    //public methods
    public static T GetReflectionValue<T>(UnityEngine.Object obj, string fieldName)
    {
        Type objType = obj.GetType();
        FieldInfo field = objType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        object value = field.GetValue(obj);
        return (T)value;
    }
    public static void SetReflectionValue(UnityEngine.Object orgObj, UnityEngine.Object targetObj, string fieldName)
    {
        Type orgObjType = orgObj.GetType();
        FieldInfo field = orgObjType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        object value = field.GetValue(orgObj);

        Type targetObjType = targetObj.GetType();
        FieldInfo targetField = targetObjType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        targetField.SetValue(targetObj, value);
    }
    public static bool HitAtCursor(out RaycastHit hit, float rayDistance, LayerMask mask)
    {
        if (MainCamera == null)
        {
            hit = default(RaycastHit);
            return false;
        }
        Ray ray = MainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        return Physics.Raycast(ray, out hit, rayDistance, mask);
    }
    public static bool HitAllAtCursor(out RaycastHit[] allHits, float rayDistance, LayerMask mask)
    {
        if (MainCamera == null)
        {
            allHits = default(RaycastHit[]);
            return false;
        }
        Ray ray = MainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        allHits = Physics.RaycastAll(ray, rayDistance, mask);
        if (allHits.Length > 0)
        {
            return true;
        }
        return false;
    }
    public static void SetCursorVisible(bool state)
    {
        Cursor.visible = state;
    }
    public static void SetCursorLockState(CursorLockMode mode)
    {
        Cursor.lockState = mode;
        lockMode = mode;
    }
    public static void SetCursorVisibleAndLockState(bool state, CursorLockMode mode)
    {
        SetCursorVisible(state);
        SetCursorLockState(mode);
    }
    public static Vector3 GetColliderExtents(Collider col)
    {
        if (col == null)
            return Vector3.zero;

        if (col.enabled)
        {
            if (!col.gameObject.activeInHierarchy)
            {
                col.gameObject.SetActive(true);
                Vector3 extents = col.bounds.extents;
                col.gameObject.SetActive(false);
                return extents;
            }
            else
                return col.bounds.extents;
        }
        else
        {
            col.enabled = true;
            Vector3 extents = col.bounds.extents;
            col.enabled = false;

            return extents;
        }
    }
    public static Vector3 GetColliderSize(BoxCollider col)
    {
        return Vector3.Scale(col.size, col.transform.lossyScale);
    }
    public static Vector3 GetColliderCenter(Collider col)
    {
        if (col == null)
            return Vector3.zero;

        if (col.enabled)
        {
            if (!col.gameObject.activeInHierarchy)
            {
                col.gameObject.SetActive(true);
                Vector3 center = col.bounds.center;
                col.gameObject.SetActive(false);

                return center;
            }
            else
                return col.bounds.center;
        }
        else
        {
            col.enabled = true;
            Vector3 center = col.bounds.center;
            col.enabled = false;

            return center;
        }
    }
    public static Collider[] GetCollisions(BoxCollider col, LayerMask mask, float extentsMultiplier = 1f)
    {
        return Physics.OverlapBox(col.transform.position + col.center, (GetColliderSize(col) * 0.5f) * extentsMultiplier, col.transform.rotation, mask);
    }
    public static Collider[] GetCollisions(BoxCollider col, LayerMask mask, BoxCollider[] excludeColliders, float extentsMultiplier = 1f)
    {
        List<Collider> collisions = Physics.OverlapBox(col.transform.position + col.center, (GetColliderSize(col) * 0.5f) * extentsMultiplier, col.transform.rotation, mask).ToList();

        for (int i = 0; i < collisions.Count; i++)
        {
            Collider hitCol = collisions[i];

            if (hitCol.transform == col.transform)
                continue;
            else
            {
                foreach (BoxCollider exCol in excludeColliders)
                {
                    if (hitCol == exCol)
                    {
                        collisions.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
        }

        return collisions.ToArray();
    }
    public static bool IsColliding(BoxCollider col, LayerMask mask, float extentsMultiplier = 1f, bool includeOwnCollider = true, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (col == null)
            return false;
        Collider[] collisions = Physics.OverlapBox(GetColliderCenter(col), (GetColliderSize(col) * 0.5f) * extentsMultiplier, col.transform.rotation, mask, triggerInteraction);
        foreach (Collider r in collisions)
        {
            if (r.transform == col.transform)
                continue;
            else
            {
                return true;
            }
        }

        return false;
    }
    public static bool IsColliding(BoxCollider col, LayerMask mask, BoxCollider[] excludeColliders, float extentsMultiplier = 1f, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        Collider[] collisions = Physics.OverlapBox(GetColliderCenter(col), (GetColliderSize(col) * 0.5f) * extentsMultiplier, col.transform.rotation, mask, triggerInteraction);
        foreach (Collider hitCol in collisions)
        {
            if (hitCol.transform == col.transform)
                continue;
            else
            {
                bool isInExcludeList = false;
                foreach (BoxCollider exCol in excludeColliders)
                {
                    if (hitCol == exCol)
                    {
                        isInExcludeList = true;
                        break;
                    }
                }
                if (isInExcludeList)
                    continue;
                else
                    return true;
            }
        }

        return false;
    }
    public static bool IsColliding(BoxCollider col, LayerMask mask, BoxCollider[] excludeColliders, out Collider hitCollider, float extentsMultiplier = 1f, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        Collider[] collisions = Physics.OverlapBox(GetColliderCenter(col), (GetColliderSize(col) * 0.5f) * extentsMultiplier, col.transform.rotation, mask, triggerInteraction);
        foreach (Collider hitCol in collisions)
        {
            if (hitCol.transform == col.transform)
                continue;
            else
            {
                bool isInExcludeList = false;
                foreach (BoxCollider exCol in excludeColliders)
                {
                    if (hitCol == exCol)
                    {
                        isInExcludeList = true;
                        break;
                    }
                }
                if (isInExcludeList)
                    continue;
                else
                {
                    hitCollider = hitCol;
                    return true;
                }
            }
        }

        hitCollider = null;
        return false;
    }
    public static Vector3 GetRandomPointAroundOrigin(Vector3 origin, float minRadius, float maxRadius)
    {
        Vector3 point = origin + UnityEngine.Random.insideUnitSphere.normalized * UnityEngine.Random.Range(minRadius, maxRadius);
        return point;
    }
    public static bool HasClearVision(Vector3 origin, Vector3 target, LayerMask mask)
    {
        Vector3 dirToPoint = target - origin;
        if (target == origin || dirToPoint.magnitude < 0.5f)
            return true;

        RaycastHit hit;

        if (Physics.Raycast(origin, dirToPoint.normalized, out hit, dirToPoint.magnitude, mask))
        {
            //Debug.DrawRay(origin, dirToPoint.normalized * rayLength, Color.red, 1f);
            return false;
        }
        else
        {
            //Debug.DrawRay(origin, dirToPoint.normalized * rayLength, Color.green, 1f);
            return true;
        }
    }
    public static bool HasClearVision(Vector3 origin, Vector3 target, LayerMask mask, float rayLength = float.PositiveInfinity)
    {
        Vector3 dirToPoint = target - origin;
        if (target == origin || dirToPoint.magnitude < 0.5f)
            return true;

        RaycastHit hit;

        if (Physics.Raycast(origin, dirToPoint.normalized, out hit, rayLength, mask))
        {
            //Debug.DrawRay(origin, dirToPoint.normalized * rayLength, Color.red, 1f);
            return false;
        }
        else
        {
            //Debug.DrawRay(origin, dirToPoint.normalized * rayLength, Color.green, 1f);
            return true;
        }
    }
    public static bool HasClearVisionAndWithinRange(Vector3 origin, Vector3 target, LayerMask mask, float visionRange, float rayLength = float.PositiveInfinity)
    {
        Vector3 dirToPoint = target - origin;
        if (target == origin || dirToPoint.magnitude < 0.5f)
            return true;

        RaycastHit hit;
        if (Physics.Raycast(origin, dirToPoint.normalized, out hit, rayLength, mask))
        {
            return false;
        }
        else
        {
            float dist = Vector3.Distance(origin, target);
            if (dist <= visionRange)
                return true;
            else
                return false;
        }
    }
    public static bool WithinDistance(Vector3 a, Vector3 b, float distance)
    {
        return Vector3.Distance(a, b) <= distance;
    }
    public static void SetLayerOfTransformAndChildren(Transform trans, string layerName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            trans.GetChild(i).gameObject.layer = LayerMask.NameToLayer(layerName);
            SetLayerOfTransformAndChildren(trans.GetChild(i), layerName);
        }
    }
    public static float Remap(float min, float max, float value, float newMin, float newMax)
    {
        return newMin + (value - min) * (newMax - newMin) / (max - min);
    }
    public static float NormalizeValue(float value, float min, float max)
    {
        return Mathf.Clamp((value - min) / (max - min), 0f, 1f);
    }
    public static Vector3 DampVector(Vector3 source, Vector3 target, float smoothing, float dt)
    {
        source.x = Mathf.Lerp(source.x, target.x, 1 - Mathf.Pow(smoothing, dt));
        source.y = Mathf.Lerp(source.y, target.y, 1 - Mathf.Pow(smoothing, dt));
        source.z = Mathf.Lerp(source.z, target.z, 1 - Mathf.Pow(smoothing, dt));
        return source;
    }
    public static float Damp(float source, float target, float smoothing, float dt)
    {
        return Mathf.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }
    public static int BoolToInt(bool value)
    {
        return value == true ? 1 : 0;
    }
    public static bool IntToBool(int value)
    {
        return value == 0 ? false : true;
    }
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
    public static Vector3 XZDirectionFromAngle(float angle)
    {
        //Angle 0 = Vector3.right
        //Angle 90 = Vector3.forward
        return new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), 0, Mathf.Sin(Mathf.Deg2Rad * angle)).normalized;
    }
    public static Vector3 GetBezierCurveValue(Vector3 start, Vector3 end, float centerHeight, float normalizedTime)
    {
        Vector3 dirToEnd = end - start;
        Vector3 center = start + (dirToEnd / 2f) + Vector3.up * centerHeight;
        Vector3 m1 = Vector3.Lerp(start, center, normalizedTime);
        Vector3 m2 = Vector3.Lerp(center, end, normalizedTime);
        return Vector3.Lerp(m1, m2, normalizedTime);
    }
    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }
    public static bool ObjectIsOnLayer(GameObject objectToCompare, LayerMask layerToCompare)
    {
        return ((1 << objectToCompare.layer) & layerToCompare) != 0;
    }
    public static Vector3 GetLocalPositionComparedToRoot(Transform root, Transform compareTransform)
    {
        Vector3 localPos = compareTransform.localPosition;

        LocalPositionComparedToRootRecursive(root, compareTransform.parent, ref localPos);

        return localPos;
    }
    public static Vector3 GetLocalEulerComparedToRoot(Transform root, Transform compareTransform)
    {
        Vector3 localEul = compareTransform.localEulerAngles;

        LocalEuleromparedToRootRecursive(root, compareTransform.parent, ref localEul);

        return localEul;
    }
    public static Vector3 GetClosestPointOnNavMesh(Vector3 sourcePosition)
    {
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(sourcePosition, out hit, 100f, UnityEngine.AI.NavMesh.AllAreas))
        {
            return hit.position;
        }
        return sourcePosition;
    }
    public static Vector3 GetClosestPointOnNavMesh(Vector3 sourcePosition, float range)
    {
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(sourcePosition, out hit, range, UnityEngine.AI.NavMesh.AllAreas))
        {
            return hit.position;
        }
        return sourcePosition;
    }
    public static void AddEventTriggerListener(UnityEngine.EventSystems.EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((eventData) => { callback(eventData); });
        trigger.triggers.Add(entry);
    }
    public static bool SamePosition(Vector3 a, Vector3 b, float marginal = 0.05f)
    {
        return Vector3.Distance(a, b) <= marginal;
    }
    public static int RandomOneMinus()
    {
        return UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
    }
    public static int OneMinusFromValue(float value)
    {
        if (value < 0)
            return -1;
        return 1;
    }
    public static T ObjToValue<T>(object obj)
    {
        if (obj != null && obj.GetType() == typeof(T))
        {
            return (T)obj;
        }
        else
        {
            return default(T);
        }
    }

    //private methods
    private static void LocalEuleromparedToRootRecursive(Transform root, Transform parent, ref Vector3 localEuler)
    {
        if (parent == null || parent == root)
        {
            return;
        }
        else
        {
            localEuler += parent.localEulerAngles;
            LocalEuleromparedToRootRecursive(root, parent.parent, ref localEuler);
        }
    }
    private static void LocalPositionComparedToRootRecursive(Transform root, Transform parent, ref Vector3 localPos)
    {
        if (parent == null || parent == root)
        {
            return;
        }
        else
        {
            localPos += parent.localPosition;
            LocalPositionComparedToRootRecursive(root, parent.parent, ref localPos);
        }
    }
}