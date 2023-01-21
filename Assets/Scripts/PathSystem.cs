using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSystem : MonoBehaviour
{
    [SerializeField]
    private float wireSphereSize = 1f;

    void OnDrawGizmos()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(t.position, wireSphereSize);
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
        Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }

    void Update()
    {
        


    }
}
