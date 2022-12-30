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
        
        public float PitchFactor; // Vector.Right
        public float RollFactor; // Vector.Foward
        public float YawFactor; // Vector.Up

        public float FlapsFactor;
        public float BreakFactor;
    }

    [SerializeField]
    public List<ControlSurface> controlSurfaces = new List<ControlSurface>();
    
    public float PitchSpeed;
    public float RollSpeed;
    public float YawSpeed;
    public float FlapsSpeed;
    public float BreakSpeed;

    private Rigidbody _rb;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    public void ApplySurfaceTorques()
    {
        
    }
}
