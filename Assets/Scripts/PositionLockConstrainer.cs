using System;
using System.Collections.Generic;
using UnityEngine;

public class PositionLockConstrainer : MonoBehaviour
{
    private Vector3 _defaultPosition;
    private Vector3 _defaultRotation;
    
    [Serializable]
    public class AxisContraints
    {
        public bool Constrain;
        public bool Inverse;
        public float MinDisplacment;
        public float MaxDisplacment;
        public List<float> lockPoints;
    }

    [SerializeField]
    private bool _constraintIsActive;
    public bool ConstraintIsActive
    {
        get => _constraintIsActive;
        set => _constraintIsActive = value;
    }

    private bool _moveIsActive;
    [SerializeField]
    private float _moveSpeed;
    
    [SerializeField]
    private GameObject _constraintSource;

    [SerializeField]
    private AxisContraints _XPosition = new AxisContraints();
    [SerializeField]
    private AxisContraints _YPosition = new AxisContraints();
    [SerializeField]
    private AxisContraints _ZPosition = new AxisContraints();

    #region AxisContraintProperties
    
        public AxisContraints XPosition => _XPosition;
        public AxisContraints YPosition => _YPosition;
        public AxisContraints ZPosition => _ZPosition;

    #endregion

    private Vector3 _targetPosition;

    void Start()
    {
        _defaultPosition = gameObject.transform.localPosition;
        _defaultRotation = gameObject.transform.localEulerAngles;
        CalculateNewTargetPosition();
    }

    void Update()
    {
        if (_constraintIsActive)
        {
            _moveIsActive = false;
            transform.localPosition = ClampedPosition(_constraintSource.transform.localPosition);
        }
        else if (_moveIsActive == false)
        {
            _moveIsActive = true;
            CalculateNewTargetPosition();
        }

        if (_moveIsActive)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, _moveSpeed * Time.deltaTime);
        }
            
    }
    
    public void InvokeReset()
    {
        gameObject.transform.localPosition = _defaultPosition;
        gameObject.transform.localEulerAngles = _defaultRotation;
    }

    Vector3 ClampedPosition(Vector3 sourcePosition)
    {    
        float posX = sourcePosition.x;
        float posY = sourcePosition.y;
        float posZ = sourcePosition.z;
        
        posX *= _XPosition.Inverse ? -1: 1;
        posY *= _YPosition.Inverse ? -1: 1;
        posZ *= _ZPosition.Inverse ? -1: 1;

        if (_XPosition.Constrain)
        {
            posX = Mathf.Clamp(posZ, _XPosition.MinDisplacment, _XPosition.MaxDisplacment);
        }
        if (_YPosition.Constrain)
        {
            posY = Mathf.Clamp(posY, _YPosition.MinDisplacment, _YPosition.MaxDisplacment);
        }
        if (_ZPosition.Constrain)
        {
            posZ = Mathf.Clamp(posZ, _ZPosition.MinDisplacment, _ZPosition.MaxDisplacment);
        }
        
        return new Vector3(posX, posY, posZ);
    }

    void CalculateNewTargetPosition()
    {
        _targetPosition = NewTargetPosition();
    }
    
    Vector3 NewTargetPosition()
    {
        float posX = transform.localPosition.x;
        float posY = transform.localPosition.y;
        float posZ = transform.localPosition.z;

        List<float> xLockPoints = _XPosition.lockPoints;
        List<float> yLockPoints = _YPosition.lockPoints;
        List<float> zLockPoints = _ZPosition.lockPoints;

        posX = CalculateClosestPoint(xLockPoints, posX);
        posY = CalculateClosestPoint(yLockPoints, posY);
        posZ = CalculateClosestPoint(zLockPoints, posZ);

        return new Vector3(posX, posY, posZ);
    }

    float CalculateClosestPoint(List<float> pointsList, float currentPoint)
    {
        if (pointsList.Count >= 2) 
        {
            pointsList.Sort();
            float upper = 0, lower = 0;
            for (int i = 0; i < pointsList.Count; i++)
            {
                float current = pointsList[i];
                if (currentPoint >= current)
                {
                    lower = current;
                    if (i == pointsList.Count - 1) upper = lower;
                }
                else
                {
                    upper = current;
                    if (i == 0) lower = upper;
                    break;
                }
            }
            currentPoint = (currentPoint >= (lower + upper) / 2f) ? upper: lower;
        }
        else if (pointsList.Count == 1)
        {
            currentPoint = pointsList[0];
        }
        return currentPoint;
    }
}
