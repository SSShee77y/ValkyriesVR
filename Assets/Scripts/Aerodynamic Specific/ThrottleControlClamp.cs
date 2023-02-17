using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrottleControlClamp : MonoBehaviour
{
    [SerializeField]
    private Transform _constraintSource;
    [SerializeField]
    private Vector3 _minPosition;
    [SerializeField]
    private Vector3 _maxPosition;
    [SerializeField]
    private Vector3 _afterburnerPosition;

    void Update()
    {
        transform.localPosition = ClampedPosition(_constraintSource.localPosition);
    }

    Vector3 ClampedPosition(Vector3 sourcePosition)
    {    
        float posX = sourcePosition.x;
        float posY = sourcePosition.y;
        float posZ = sourcePosition.z;

        posX = (posX < _afterburnerPosition.x) ? Mathf.Clamp(posX, _minPosition.x, _maxPosition.x) : _afterburnerPosition.x;
        posY = (posY < _afterburnerPosition.y) ? Mathf.Clamp(posY, _minPosition.y, _maxPosition.y) : _afterburnerPosition.y;
        posZ = (posZ < _afterburnerPosition.z) ? Mathf.Clamp(posZ, _minPosition.z, _maxPosition.z) : _afterburnerPosition.z;
        
        return new Vector3(posX, posY, posZ);
    }
}