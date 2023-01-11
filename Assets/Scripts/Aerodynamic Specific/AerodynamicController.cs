using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AerodynamicController : MonoBehaviour
{
    [Serializable]
    public class ControlSurface
    {
        public Transform transform;
        public enum SurfaceType
        {
            LeftAileron,
            RightAileron,
            Elevator,
            Stabilizer,
            Flaps,
            Brake
        }
        public SurfaceType surfaceType;
        
        public float Factor;
    }
    [SerializeField]
    private float _startingSpeed;
    [SerializeField]
    private GameObject _throttleControl;
    [SerializeField]
    private float _maximumThrottleDisplacement;
    private float _minimumThrottleDisplacement;
    
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
    private float _maxFacingArea = 250f;
    [SerializeField]
    private float _fowardAreaFactor = 0.0019f;
    [SerializeField]
    private float _stallSpeed = 60f;
    [SerializeField]
    private float _stallFactor = 2f;

    [SerializeField]
    private bool _engineTest = false;

    [SerializeField]
    private List<ControlSurface> _controlSurfaces = new List<ControlSurface>();
    public List<ControlSurface> ControlSurfaces => _controlSurfaces;
    
    private Rigidbody _rb;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        float velX = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.right) * Mathf.Deg2Rad) * _startingSpeed;
        float velY = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.up) * Mathf.Deg2Rad) * _startingSpeed;
        float velZ = Mathf.Cos(Vector3.Angle(transform.forward, Vector3.forward) * Mathf.Deg2Rad) * _startingSpeed;
        _rb.velocity = new Vector3(velX, velY, velZ);

        if (_throttleControl != null) _minimumThrottleDisplacement = _throttleControl.transform.localPosition.z;
    }

    void FixedUpdate()
    {
        ApplyThrust();
        ApplyDragLift();
    }

    void ApplyThrust()
    {
        float enginePercentage = 100;
        if (_engineTest == false && _throttleControl != null)
            enginePercentage = (_throttleControl.transform.localPosition.z - _minimumThrottleDisplacement) / _maximumThrottleDisplacement * 100;

        if (_currentEngineSpeed >= enginePercentage - _engineIncreaseSpeed/2 && enginePercentage <= enginePercentage + _engineIncreaseSpeed/2)
            _currentEngineSpeed = enginePercentage;
        else if (_currentEngineSpeed > enginePercentage)
            _currentEngineSpeed -= _engineIncreaseSpeed;
        else
            _currentEngineSpeed += _engineIncreaseSpeed;

        float thrustAmount = CalculateEnginePropulsion(_currentEngineSpeed) * _thrustFactor;
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
        float dragUp = VelAngleDiffMultiplier(transform.up, 1.0f, 1.0f) * dragAmount;
        float dragFoward = VelAngleDiffMultiplier(transform.forward, _fowardAreaFactor, 2) * dragAmount;
        float dragRight = VelAngleDiffMultiplier(transform.right, 0.96f, 0.96f) * dragAmount;

        // Force Applications
        _rb.AddRelativeForce(Vector3.up * dragUp);
        _rb.AddRelativeForce(Vector3.forward * dragFoward);
        _rb.AddRelativeForce(Vector3.right * dragRight);
        ApplySurfaceTorques(dragAmount / _maxFacingArea);
    }

    float GetAngleOfAttack()
    {
        float angleOfAttack = Vector3.Angle(transform.forward, Vector3.Normalize(_rb.velocity));
        angleOfAttack *= (transform.forward.y < Vector3.Normalize(_rb.velocity).y) ? -1f: 1f;
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

    void ApplySurfaceTorques(float dragAmount)
    {
        foreach (ControlSurface controlSurface in _controlSurfaces)
        {
            if (controlSurface.surfaceType == ControlSurface.SurfaceType.LeftAileron)
            {
                float torqueAmount = -1f * VelAngleDiffMultiplier(controlSurface.transform.up) * dragAmount * controlSurface.Factor;
                _rb.AddRelativeTorque(Vector3.back * torqueAmount);
                float forceAmount = Mathf.Cos(Vector3.Angle(controlSurface.transform.up, transform.forward * -1f) * Mathf.Deg2Rad) * torqueAmount;
                _rb.AddRelativeForce(Vector3.forward * forceAmount / 2);
            }

            else if (controlSurface.surfaceType == ControlSurface.SurfaceType.RightAileron)
            {
                float torqueAmount = -1f * VelAngleDiffMultiplier(controlSurface.transform.up) * dragAmount * controlSurface.Factor;
                _rb.AddRelativeTorque(Vector3.forward * torqueAmount);
                float forceAmount = Mathf.Cos(Vector3.Angle(controlSurface.transform.up, transform.forward * -1f) * Mathf.Deg2Rad) * torqueAmount;
                _rb.AddRelativeForce(Vector3.forward * forceAmount / 2);
            }

            else if (controlSurface.surfaceType == ControlSurface.SurfaceType.Elevator)
            {
                float torqueAmount = -1f * VelAngleDiffMultiplier(controlSurface.transform.up) * dragAmount * controlSurface.Factor;
                _rb.AddRelativeTorque(Vector3.left * torqueAmount);
                float forceAmount = Mathf.Cos(Vector3.Angle(controlSurface.transform.up, transform.forward * -1f) * Mathf.Deg2Rad) * torqueAmount;
                _rb.AddRelativeForce(Vector3.forward * forceAmount / 2);

                // Apply fake lift torque
                float liftTorqueAmount = VelAngleDiffMultiplier(transform.up) * dragAmount * controlSurface.Factor;
                if (GetAngleOfAttack() >= Mathf.Abs(2.5f) && _rb.velocity.magnitude <= _stallSpeed) // If at stall speed
                {
                    /*
                     * TO-DO
                     * Maybe make stall faster depending on AoA
                     */
                    float multiplier = _stallFactor * Mathf.Cos(.5f * (Vector3.Angle(transform.forward, Vector3.Normalize(_rb.velocity)) - 2.5f) * Mathf.Deg2Rad);
                    liftTorqueAmount = multiplier * dragAmount * controlSurface.Factor * Mathf.Min(100f, Mathf.Pow(_stallSpeed / _rb.velocity.magnitude, 2));
                    liftTorqueAmount *= VelAngleDiffMultiplier(transform.up) <= VelAngleDiffMultiplier(transform.up * -1f) ? 1f: -1f;
                }
                _rb.AddRelativeTorque(Vector3.left * liftTorqueAmount);
                
            }

            else if (controlSurface.surfaceType == ControlSurface.SurfaceType.Stabilizer)
            {
                float torqueAmount = VelAngleDiffMultiplier(controlSurface.transform.right) * dragAmount * controlSurface.Factor;
                _rb.AddRelativeTorque(Vector3.down * torqueAmount);
                float forceAmount = Mathf.Abs(Mathf.Cos(Vector3.Angle(controlSurface.transform.right, transform.forward)) * Mathf.Deg2Rad) * torqueAmount;
                _rb.AddRelativeForce(Vector3.back * forceAmount / 2);
            }

            else if (controlSurface.surfaceType == ControlSurface.SurfaceType.Flaps)
            {
                if (controlSurface.transform.localEulerAngles.x != 0) {
                    float torqueAmount = VelAngleDiffMultiplier(controlSurface.transform.up) * dragAmount * controlSurface.Factor;
                    _rb.AddRelativeForce(controlSurface.transform.up * torqueAmount);
                }
            }

            else if (controlSurface.surfaceType == ControlSurface.SurfaceType.Brake)
            {
                float torqueAmount = -1f * VelAngleDiffMultiplier(controlSurface.transform.up) * dragAmount * controlSurface.Factor;
                float forceAmount = Mathf.Cos(Vector3.Angle(controlSurface.transform.up, transform.forward * -1f) * Mathf.Deg2Rad) * torqueAmount;
                _rb.AddRelativeForce(Vector3.forward * forceAmount);
            }
        }
    }
}
