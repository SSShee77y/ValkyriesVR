using System;
using UnityEngine;

public class TransformConstrainer : MonoBehaviour
{
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    
    [Serializable]
    public class AxisContraints
    {
        public bool Constrain;
        public float MinAngle;
        public float MaxAngle;
    }

    [SerializeField]
    private bool _isActive;
    [SerializeField]
    private GameObject _constraintSource;

    [SerializeField]
    private AxisContraints _XAxisRotation = new AxisContraints();
    [SerializeField]
    private AxisContraints _YAxisRotation = new AxisContraints();
    [SerializeField]
    private AxisContraints _ZAxisRotation = new AxisContraints();

    void Start()
    {
        defaultPosition = gameObject.transform.localPosition;
        defaultRotation = gameObject.transform.localEulerAngles;
    }

    void Update()
    {
        if (_isActive)
            UpdateRotation();
    }
    
    public void InvokeReset()
    {
        gameObject.transform.localPosition = defaultPosition;
        gameObject.transform.localEulerAngles = defaultRotation;
    }
    
    void UpdateRotation()
    {
        transform.localEulerAngles = LockedRotation(_constraintSource.transform.localEulerAngles);
    }

    Vector3 LockedRotation(Vector3 sourceRotation)
    {    
        float rotationX = sourceRotation.x <= 180 ? sourceRotation.x : sourceRotation.x - 360;
        float rotationY = sourceRotation.y <= 180 ? sourceRotation.y : sourceRotation.y - 360;
        float rotationZ = sourceRotation.z <= 180 ? sourceRotation.z : sourceRotation.z - 360;

        if (_XAxisRotation.Constrain)
        {
            rotationX = Mathf.Clamp(rotationX, _XAxisRotation.MinAngle, _XAxisRotation.MaxAngle);
        }
        if (_YAxisRotation.Constrain)
        {
            rotationY = Mathf.Clamp(rotationY, _YAxisRotation.MinAngle, _YAxisRotation.MaxAngle);
        }
        if (_ZAxisRotation.Constrain)
        {
            rotationZ = Mathf.Clamp(rotationZ, _ZAxisRotation.MinAngle, _ZAxisRotation.MaxAngle);
        }
        
        return new Vector3(rotationX, rotationY, rotationZ);
    }

    
}
