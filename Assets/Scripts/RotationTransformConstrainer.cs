using System;
using UnityEngine;

public class RotationTransformConstrainer : MonoBehaviour
{
    private Vector3 _defaultPosition;
    private Vector3 _defaultRotation;
    
    [Serializable]
    public class AxisContraints
    {
        public bool Constrain;
        public bool Inverse;
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

    #region AxisContraintProperties
    
        public AxisContraints XAxisRotation => _XAxisRotation;
        public AxisContraints YAxisRotation => _YAxisRotation;
        public AxisContraints ZAxisRotation => _ZAxisRotation;

    #endregion

    void Start()
    {
        _defaultPosition = gameObject.transform.localPosition;
        _defaultRotation = gameObject.transform.localEulerAngles;
    }

    void Update()
    {
        if (_isActive)
            transform.localEulerAngles = LockedRotation(_constraintSource.transform.localEulerAngles);
    }
    
    public void InvokeReset()
    {
        gameObject.transform.localPosition = _defaultPosition;
        gameObject.transform.localEulerAngles = _defaultRotation;
    }

    Vector3 LockedRotation(Vector3 sourceRotation)
    {    
        float rotationX = sourceRotation.x <= 180 ? sourceRotation.x : sourceRotation.x - 360;
        float rotationY = sourceRotation.y <= 180 ? sourceRotation.y : sourceRotation.y - 360;
        float rotationZ = sourceRotation.z <= 180 ? sourceRotation.z : sourceRotation.z - 360;
        
        rotationX *= _XAxisRotation.Inverse ? -1: 1;
        rotationY *= _YAxisRotation.Inverse ? -1: 1;
        rotationZ *= _ZAxisRotation.Inverse ? -1: 1;

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
