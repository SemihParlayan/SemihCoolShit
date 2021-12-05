using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GizmoBox : MonoBehaviour
{
    //public variables
    public virtual Vector3 Center{ get { return transform.position; }}

    public Vector3 size = Vector3.one;

    //private variables


    //unity methods
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(Vector3.zero, size);
    }

    //public methods
    public bool IsColliding(bool includeInactiveObjects, LayerMask mask, float extentsMultiplier, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        IEnumerable<Collider> overlaps = GetOverlaps(includeInactiveObjects, mask, extentsMultiplier, queryTriggerInteraction);

        return overlaps != null && overlaps.Count() > 0;

    }
    public IEnumerable<Collider> GetOverlaps(bool includeInactiveObjects,  LayerMask mask, float extentsMultiplier, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        Vector3 extents = (Vector3.Scale(size, transform.lossyScale) * 0.5f);
        Collider[] overlaps = Physics.OverlapBox(Center, extents * extentsMultiplier, transform.rotation, mask, queryTriggerInteraction);
        if (includeInactiveObjects)
        {
            return overlaps;
        }
        else
        {
            List<Collider> overlapsList = overlaps.ToList();
            for (int i = 0; i < overlapsList.Count; i++)
            {
                Collider col = overlapsList[i];
                if (!col.enabled || !col.gameObject.activeSelf)
                {
                    overlapsList.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            return overlapsList;
        }
    }
    public List<T> GetOverlaps<T>()
    {
        Vector3 extents = (Vector3.Scale(size, transform.lossyScale) * 0.5f);
        Collider[] overlaps = Physics.OverlapBox(Center, extents, transform.rotation);

        List<T> overlapsList = new List<T>();
        for (int i = 0; i < overlaps.Length; i++)
        {
            Collider col = overlaps[i];

            T script = col.GetComponent<T>();
            if (script != null)
            {
                overlapsList.Add(script);
                continue;
            }
        }
        return overlapsList;
    }

    //private methods
}