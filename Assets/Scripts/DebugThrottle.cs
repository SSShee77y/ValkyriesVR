using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugThrottle : MonoBehaviour
{
    [SerializeField]
    private GameObject throttleControl;
    [SerializeField]
    private float maximumDisplacement;
    private float minimumDisplacement;
    
    [SerializeField]
    private float thrustFactor = 40f;
    [SerializeField]
    private float currentEngineSpeed;
    [SerializeField]
    private float engineIncreaseSpeed = 0.5f;
    [SerializeField]
    private float dragFactor = 1;

    [SerializeField]
    private Transform centerOfLift;
    
    private Rigidbody _rb;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        minimumDisplacement = throttleControl.transform.position.z;
    }

    void FixedUpdate()
    {
        ApplyThrust();
        ApplyDragLift();
    }

    void ApplyThrust()
    {
        float enginePercentage = (throttleControl.transform.position.z - minimumDisplacement) / maximumDisplacement * 100;
        enginePercentage = 100;
        if (currentEngineSpeed >= enginePercentage - engineIncreaseSpeed/2 && enginePercentage <= enginePercentage + engineIncreaseSpeed/2)
            currentEngineSpeed = enginePercentage;
        else if (currentEngineSpeed > enginePercentage)
            currentEngineSpeed -= engineIncreaseSpeed;
        else
            currentEngineSpeed += engineIncreaseSpeed;

        float thrustAmount = CalculateEnginePropulsion(currentEngineSpeed) * thrustFactor;
        _rb.AddRelativeForce(Vector3.forward * thrustAmount);
    }

    float CalculateEnginePropulsion(float throttlePercentage)
    {
        if (throttlePercentage <= 20)
            return 0f;
        if (throttlePercentage <= 30)
            return 54 * (throttlePercentage - 20);
        if (throttlePercentage <= 50)
            return 540f;
        if (throttlePercentage <= 60)
            return 540f + 6 * (throttlePercentage - 50);
        if (throttlePercentage <= 80)
            return 600f + 30 * (throttlePercentage - 60);
        if (throttlePercentage <= 95)
            return 1200f + 15 * (throttlePercentage - 80);
        if (throttlePercentage <= 100)
            return 1425f + 55 * (throttlePercentage - 95);
        return 1700f + 40 * (throttlePercentage - 100);
    }

    void ApplyDragLift()
    {
        float airDensity = 1.3f;
        
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

        // Drag Math
        float facingArea = 200f;
        float dragAmount = -1 * Mathf.Pow(_rb.velocity.magnitude, 2) / 2 * airDensity * facingArea * dragCoefficient * dragFactor;
        float dragUp = VelAngleDiffMultiplier(transform.up, 1, 1) * dragAmount;
        float dragFoward = VelAngleDiffMultiplier(transform.forward, 1f/70, 5) * dragAmount;
        float dragRight = VelAngleDiffMultiplier(transform.right, 1, 1) * dragAmount;

        // Force Applications
        _rb.AddRelativeForce(Vector3.up * dragUp);
        _rb.AddRelativeForce(Vector3.forward * dragFoward);
        _rb.AddRelativeForce(Vector3.right * dragRight);
    }

    float GetAngleOfAttack()
    {
        float angleOfAttack = Vector3.Angle(transform.forward, Vector3.Normalize(_rb.velocity));
        angleOfAttack *= (transform.forward.y < Vector3.Normalize(_rb.velocity).y) ? -1f: 1f;
        return angleOfAttack;
    }

    float VelAngleDiffMultiplier(Vector3 transformDirection, float positiveEffect, float negativeEffect)
    {
        float angleDifference = Vector3.Angle(transformDirection, Vector3.Normalize(_rb.velocity));
        float multiplier = Mathf.Cos(angleDifference * Mathf.Deg2Rad);
        multiplier *= (angleDifference > 90) ? negativeEffect : positiveEffect;
        return multiplier;
    }
}
