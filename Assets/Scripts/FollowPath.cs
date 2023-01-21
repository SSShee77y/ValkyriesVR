using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField]
    private float _wireSphereSize = 1f;
    [SerializeField]
    private bool _loopPath;

    void OnDrawGizmos()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(t.position, _wireSphereSize);
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
        if (_loopPath)
            Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }

    public Transform GetFirstPoint()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();

        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i] != gameObject.transform)
            {
                return transforms[i];
            }
        }
        return null;
    }

    public Transform GetNextPoint(Transform t)
    {
        if (t == null) return null;

        Transform[] transforms = GetComponentsInChildren<Transform>();

        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i] == t)
            {
                if ((i + 1) < transforms.Length)
                    return transforms[i + 1];
                else
                    break;
            }
        }
        if (_loopPath)
            return GetFirstPoint();
        return null;
    }
}
