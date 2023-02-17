using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlToFlaps : MonoBehaviour
{
    [SerializeField]
    private Transform _sourceTransform;

    [SerializeField]
    private float _rotationSpeed = 1f;

    [Serializable]
    public class TransformToDegrees {
        public Vector3 controlPosition;

        public Vector3 degrees;
    }

    [SerializeField]
    private List<TransformToDegrees> _controlList;

    void Update()
    {
        foreach (TransformToDegrees control in _controlList)
        {
            if (_sourceTransform.localPosition.Equals(control.controlPosition))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(control.degrees), _rotationSpeed * Time.deltaTime);
            }
        }
    }
}
