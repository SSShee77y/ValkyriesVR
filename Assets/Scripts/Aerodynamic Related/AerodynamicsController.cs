using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AerodynamicsController : MonoBehaviour
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
            Break
        }
        public SurfaceType surfaceType;
        
        public float PitchFactor; // Vector.Right
        public float RollFactor; // Vector.Foward
        public float YawFactor; // Vector.Up
        
        public float FlapFactor;
        public float BreakFactor;
    }

    [SerializeField]
    private List<ControlSurface> controlSurfaces = new List<ControlSurface>();

    private Rigidbody _rb;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ApplySurfaceTorques();
    }

    public void ApplySurfaceTorques()
    {
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

        foreach (ControlSurface controlSurface in controlSurfaces)
        {
            _rb.AddForceAtPosition(controlSurface.transform.up * -VelAngleDiffMultiplier(controlSurface.transform.up) * controlSurface.PitchFactor
                    * Mathf.Pow(_rb.velocity.magnitude, 2) / 2 * dragCoefficient, controlSurface.transform.position);
            _rb.AddForceAtPosition(controlSurface.transform.right * -VelAngleDiffMultiplier(controlSurface.transform.right) * controlSurface.RollFactor
                    * Mathf.Pow(_rb.velocity.magnitude, 2) / 2 * dragCoefficient, controlSurface.transform.position);
            _rb.AddForceAtPosition(controlSurface.transform.forward * -VelAngleDiffMultiplier(controlSurface.transform.forward) * controlSurface.YawFactor
                    * Mathf.Pow(_rb.velocity.magnitude, 2) / 2 * dragCoefficient, controlSurface.transform.position);
        }
    }

    float VelAngleDiffMultiplier(Vector3 transformDirection)
    {
        float angleDifference = Vector3.Angle(transformDirection, Vector3.Normalize(_rb.velocity));
        float multiplier = Mathf.Cos(angleDifference * Mathf.Deg2Rad);
        return multiplier;
    }
}
