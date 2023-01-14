using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Aerodynamics : MonoBehaviour
{
    [SerializeField]
    private float _startingSpeed;
    
    [SerializeField]
    private float _thrustFactor = 40f;
    [SerializeField]
    private float _currentEngineSpeed;
    public float CurrentEngineSpeed => _currentEngineSpeed;
    [SerializeField]
    private float _engineIncreaseSpeed = 0.5f;
    [SerializeField]
    private float _dragFactor = 1f;
    [SerializeField]
    private float _maxFacingArea = 200f;
    [SerializeField]
    private float _fowardAreaFactor = 0.0025f;
    [SerializeField]
    private float _upAreaFactor = 1.0f;
    [SerializeField]
    private float _rightAreaFactor = 1.0f;
    
    private Rigidbody _rb;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        SetSpeed(_startingSpeed);
    }

    protected void SetSpeed(float speed)
    {
        float velX = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.right) * Mathf.Deg2Rad) * speed;
        float velY = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.up) * Mathf.Deg2Rad) * speed;
        float velZ = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.forward) * Mathf.Deg2Rad) * speed;
        _rb.velocity = new Vector3(velX, velY, velZ);
    }

    void FixedUpdate()
    {
        ApplyThrust();
        ApplyDragLift();
    }

    void ApplyThrust()
    {
        float enginePercentage = 100;
        if (enginePercentage <= 0)
            enginePercentage = 0;

        if (_currentEngineSpeed >= enginePercentage - _engineIncreaseSpeed/2 && _currentEngineSpeed <= enginePercentage + _engineIncreaseSpeed/2)
            _currentEngineSpeed = enginePercentage;
        else if (_currentEngineSpeed > enginePercentage)
            _currentEngineSpeed -= _engineIncreaseSpeed;
        else
            _currentEngineSpeed += _engineIncreaseSpeed;

        float thrustAmount = _currentEngineSpeed * _thrustFactor;
        _rb.AddRelativeForce(Vector3.forward * thrustAmount);
    }

    void ApplyDragLift()
    {
        float airDensity = Mathf.Pow(1.1068f, 2f - 0.788f * Mathf.Pow(transform.position.y / 1000f, 1.15f)); // Close approximation
        
        // Drag Coefficient Formula
        float dragCoefficient = 1.63f;
        if (_rb.velocity.magnitude <= 237)
        {
            dragCoefficient = (float) (0.9940 + 0.000005 * Mathf.Pow((_rb.velocity.magnitude - 237), 2));
        }
        else if (_rb.velocity.magnitude >= 408)
        {
            dragCoefficient = (float) (2.2674 - 0.0000025 * Mathf.Pow((_rb.velocity.magnitude - 408), 2));
        }
        else
        {
            dragCoefficient = (float) (1.63 + .54 * Mathf.Pow((_rb.velocity.magnitude - 320), 1/27));
        }

        // Set Angular Drag
        _rb.angularDrag = 1 + dragCoefficient * airDensity * Mathf.Pow(_rb.velocity.magnitude, 2) / 40000;

        // Drag Math
        float dragAmount = -1 * Mathf.Pow(_rb.velocity.magnitude, 2) / 2 * airDensity * _maxFacingArea * dragCoefficient * _dragFactor;
        float dragUp = VelAngleDiffMultiplier(transform.up, _upAreaFactor, _upAreaFactor) * dragAmount;
        float dragFoward = VelAngleDiffMultiplier(transform.forward, _fowardAreaFactor, 2) * dragAmount;
        float dragRight = VelAngleDiffMultiplier(transform.right, _rightAreaFactor, _rightAreaFactor) * dragAmount;

        // Force Applications
        _rb.AddRelativeForce(Vector3.up * dragUp);
        _rb.AddRelativeForce(Vector3.forward * dragFoward);
        _rb.AddRelativeForce(Vector3.right * dragRight);
    }

    float GetAngleOfAttack()
    {
        float angleOfAttack = Vector3.Angle(transform.forward, Vector3.Normalize(_rb.velocity));
        //angleOfAttack *= (transform.forward.y < Vector3.Normalize(_rb.velocity).y) ? -1f: 1f;
        return angleOfAttack;
    }

    float VelAngleDiffMultiplier(Vector3 transformDirection)
    {
        return VelAngleDiffMultiplier(transformDirection, 1, 1);
    }

    float VelAngleDiffMultiplier(Vector3 transformDirection, float positiveEffect, float negativeEffect)
    {
        float angleDifference = Vector3.Angle(transformDirection, Vector3.Normalize(_rb.velocity));
        float multiplier = Mathf.Cos(angleDifference * Mathf.Deg2Rad);
        multiplier *= (angleDifference < 90) ? positiveEffect : negativeEffect;
        return multiplier;
    }
}