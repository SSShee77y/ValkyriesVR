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
    public AxisContraints XPosition = new AxisContraints();
    [SerializeField]
    public AxisContraints YPosition = new AxisContraints();
    [SerializeField]
    public AxisContraints ZPosition = new AxisContraints();

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
        
        posX *= XPosition.Inverse ? -1: 1;
        posY *= YPosition.Inverse ? -1: 1;
        posZ *= ZPosition.Inverse ? -1: 1;

        if (XPosition.Constrain)
        {
            posX = Mathf.Clamp(posX, XPosition.MinDisplacment, XPosition.MaxDisplacment);
        }
        if (YPosition.Constrain)
        {
            posY = Mathf.Clamp(posY, YPosition.MinDisplacment, YPosition.MaxDisplacment);
        }
        if (ZPosition.Constrain)
        {
            posZ = Mathf.Clamp(posZ, ZPosition.MinDisplacment, ZPosition.MaxDisplacment);
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

        List<float> xLockPoints = XPosition.lockPoints;
        List<float> yLockPoints = YPosition.lockPoints;
        List<float> zLockPoints = ZPosition.lockPoints;

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
