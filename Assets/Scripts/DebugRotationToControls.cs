using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRotationToControls : MonoBehaviour
{
    private Vector3 _defaultRotation;

    [SerializeField]
    private DebugUIController _controllerX;
    [SerializeField]
    private DebugUIController _controllerY;
    [SerializeField]
    private DebugUIController _controllerZ;
    
    void Start()
    {
        _defaultRotation = transform.localEulerAngles;
    }

    void Update()
    {
        if (_controllerX != null)
            _controllerX.SliderValue = transform.localEulerAngles.x <= 180 ? transform.localEulerAngles.x : transform.localEulerAngles.x - 360;
        if (_controllerY != null)
            _controllerY.SliderValue = transform.localEulerAngles.y <= 180 ? transform.localEulerAngles.y : transform.localEulerAngles.y - 360;
        if (_controllerZ != null)
            _controllerZ.SliderValue = transform.localEulerAngles.z <= 180 ? transform.localEulerAngles.z : transform.localEulerAngles.z - 360;
    }
}
