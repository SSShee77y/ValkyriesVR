using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Aerodynamics : MonoBehaviour
{
    [SerializeField]
    protected float _startingSpeed;
    
    [Header("Thrust Attributes")]
    [SerializeField]
    protected float _thrustFactor = 40f;
    [SerializeField]
    protected float _currentEngineSpeed;
    public float CurrentEngineSpeed => _currentEngineSpeed;
    [SerializeField]
    protected float _engineIncreaseSpeed = 0.5f;
    
    [Header("Drag Attributes")]
    [SerializeField]
    protected float _dragFactor = 1f;
    [SerializeField]
    protected float _maxFacingArea = 100f;
    [SerializeField]
    protected float _fowardAreaFactor = 0.0025f;
    [SerializeField]
    protected float _backwardAreaFactor = 1.0f;
    [SerializeField]
    protected float _upAreaFactor = 1.0f;
    [SerializeField]
    protected float _rightAreaFactor = 1.0f;
    
    protected Rigidbody _rb;

    protected virtual void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        SetSpeed(_startingSpeed);
    }

    protected virtual void SetSpeed(float speed)
    {
        float velX = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.right) * Mathf.Deg2Rad) * speed;
        float velY = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.up) * Mathf.Deg2Rad) * speed;
        float velZ = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.forward) * Mathf.Deg2Rad) * speed;
        _rb.velocity = new Vector3(velX, velY, velZ);
    }

    protected virtual void FixedUpdate()
    {
        ApplyThrust();
        ApplyDragLift();
    }

    protected virtual void ApplyThrust()
    {
        float enginePercentage = 100;

        if (_currentEngineSpeed >= enginePercentage - _engineIncreaseSpeed/2 && _currentEngineSpeed <= enginePercentage + _engineIncreaseSpeed/2)
            _currentEngineSpeed = enginePercentage;
        else if (_currentEngineSpeed > enginePercentage)
            _currentEngineSpeed -= _engineIncreaseSpeed;
        else
            _currentEngineSpeed += _engineIncreaseSpeed;

        float thrustAmount = _currentEngineSpeed * _thrustFactor;
        _rb.AddRelativeForce(Vector3.forward * thrustAmount);
    }

    protected virtual void ApplyDragLift()
    {
        float dragAmount = DragAmountMath();
        float dragUp = VelAngleDiffMultiplier(transform.up, _upAreaFactor, _upAreaFactor) * dragAmount;
        float dragFoward = VelAngleDiffMultiplier(transform.forward, _fowardAreaFactor, _backwardAreaFactor) * dragAmount;
        float dragRight = VelAngleDiffMultiplier(transform.right, _rightAreaFactor, _rightAreaFactor) * dragAmount;

        // Force Applications
        _rb.AddRelativeForce(Vector3.up * dragUp);
        _rb.AddRelativeForce(Vector3.forward * dragFoward);
        _rb.AddRelativeForce(Vector3.right * dragRight);
        AdditionalDragMethods(dragAmount);
    }

    protected virtual float DragAmountMath()
    {
        float heightDisplacement = FindObjectOfType<MoveToOrigin>().HeightDisplacement;
        float airDensity = Mathf.Pow(1.1068f, 2f - 0.788f * Mathf.Pow((heightDisplacement + transform.position.y) / 1000f, 1.15f)); // Close approximation
        
        // Drag Coefficient Formula
        float dragCoefficient = 1.63f;
        if (_rb.velocity.magnitude <= 237)
        {
            dragCoefficient = (float) (0.9940 + 0.000005 * Mathf.Pow((_rb.velocity.magnitude - 237), 2));
        }
        else if (_rb.velocity.magnitude >= 408)
        {
            dragCoefficient = (float) (2.2674 - 0.0000025 * Mathf.Pow((_rb.velocity.magnitude - 408), 2));
            if (dragCoefficient < 2) dragCoefficient = 2f;
        }
        else
        {
            dragCoefficient = (float) (1.63 + .54 * Mathf.Pow((_rb.velocity.magnitude - 320), 1/27));
        }

        // Set Angular Drag
        _rb.angularDrag = 1 + dragCoefficient * airDensity * Mathf.Pow(_rb.velocity.magnitude, 2) / 40000;

        // Drag Math
        float dragAmount = -1f * Mathf.Pow(_rb.velocity.magnitude, 2) / 2f * airDensity * _maxFacingArea * dragCoefficient * _dragFactor;
        return dragAmount;
    }

    protected virtual void AdditionalDragMethods(float dragAmount) {}

    public float GetAngleOfAttack()
    {
        float angleOfAttack = Vector3.Angle(transform.forward, Vector3.Normalize(_rb.velocity));
        //angleOfAttack *= (transform.forward.y < Vector3.Normalize(_rb.velocity).y) ? -1f: 1f;
        return angleOfAttack;
    }

    protected float VelAngleDiffMultiplier(Vector3 transformDirection)
    {
        return VelAngleDiffMultiplier(transformDirection, 1, 1);
    }

    protected float VelAngleDiffMultiplier(Vector3 transformDirection, float positiveEffect, float negativeEffect)
    {
        float angleDifference = Vector3.Angle(transformDirection, Vector3.Normalize(_rb.velocity));
        float multiplier = Mathf.Cos(angleDifference * Mathf.Deg2Rad);
        multiplier *= (angleDifference < 90) ? positiveEffect : negativeEffect;
        return multiplier;
    }

    public void SetEngineThrust(float thrust)
    {
        _thrustFactor = thrust;
    }
}