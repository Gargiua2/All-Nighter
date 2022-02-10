using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    public List<Transform> NPCIdlePoints;
    public List<Transform> doorEntryPoints;

    void OnValidate()
    {
        foreach(Transform t in NPCIdlePoints)
        {
            RaycastHit hit;

            if(Physics.Raycast(new Ray(t.transform.position + Vector3.up*.1f, Vector3.down), out hit,100f))
            {
                t.position = hit.point + Vector3.up * .1f;
            }
        }

        foreach (Transform t in doorEntryPoints)
        {
            RaycastHit hit;

            if (Physics.Raycast(new Ray(t.transform.position + Vector3.up * .1f, Vector3.down), out hit, 100f))
            {
                t.position = hit.point + Vector3.up * .1f;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach(Transform t in NPCIdlePoints)
        {
            Gizmos.color = Color.red * .5f;
            Gizmos.DrawSphere(t.position, 3f);
        }

        foreach (Transform t in doorEntryPoints)
        {
            Gizmos.color = Color.blue * .5f;
            Gizmos.DrawSphere(t.position, 3f);
        }
    }
}
