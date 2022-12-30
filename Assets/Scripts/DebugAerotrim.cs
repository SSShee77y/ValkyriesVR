using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DebugAerotrim : MonoBehaviour
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
            Stabilizer
        }
        public SurfaceType surfaceType;
        public float RollFactor;
        public float PitchFactor;
        public float YawFactor;
    }

    [SerializeField]
    public List<ControlSurface> ControlSurfaces = new List<ControlSurface>();

    private Rigidbody _rb;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        ApplySurfaceTorques();
    }

    public void ApplySurfaceTorques()
    {
        foreach (ControlSurface controlSurface in ControlSurfaces)
        {
            if (controlSurface.surfaceType == ControlSurface.SurfaceType.LeftAileron)
            {
                float torqueAmount = controlSurface.transform.localEulerAngles.x <= 180 ?
                        controlSurface.transform.localEulerAngles.x : controlSurface.transform.localEulerAngles.x - 360;
                torqueAmount *= controlSurface.RollFactor;
                _rb.AddRelativeTorque(Vector3.back * torqueAmount);
            }

            else if (controlSurface.surfaceType == ControlSurface.SurfaceType.RightAileron)
            {
                float torqueAmount = controlSurface.transform.localEulerAngles.x <= 180 ?
                        controlSurface.transform.localEulerAngles.x : controlSurface.transform.localEulerAngles.x - 360;
                torqueAmount *= controlSurface.RollFactor;
                _rb.AddRelativeTorque(Vector3.forward * torqueAmount);
            }

            else if (controlSurface.surfaceType == ControlSurface.SurfaceType.Elevator)
            {
                float torqueAmount = controlSurface.transform.localEulerAngles.x <= 180 ?
                        controlSurface.transform.localEulerAngles.x : controlSurface.transform.localEulerAngles.x - 360;
                torqueAmount *= controlSurface.PitchFactor;
                _rb.AddRelativeTorque(Vector3.left * torqueAmount);
            }//*/

            else if (controlSurface.surfaceType == ControlSurface.SurfaceType.Stabilizer)
            {
                float torqueAmount = controlSurface.transform.localEulerAngles.y <= 180 ?
                        controlSurface.transform.localEulerAngles.y : controlSurface.transform.localEulerAngles.y - 360;
                torqueAmount *= controlSurface.YawFactor;
                _rb.AddRelativeTorque(Vector3.down * torqueAmount);
            }//*/
        }
    }

}
