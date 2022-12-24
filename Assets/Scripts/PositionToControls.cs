using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionToControls : MonoBehaviour
{
    private Vector3 _defaultPosition;

    [SerializeField]
    private DebugUIController _controllerX;
    [SerializeField]
    private DebugUIController _controllerY;
    [SerializeField]
    private DebugUIController _controllerZ;

    void Start()
    {
        _defaultPosition = transform.localPosition;
    }

    void Update()
    {
        if (_controllerX != null)
            _controllerX.SliderValue = transform.localPosition.x - _defaultPosition.x;
        if (_controllerY != null)
            _controllerY.SliderValue = transform.localPosition.y - _defaultPosition.y;
        if (_controllerZ != null)
            _controllerZ.SliderValue = transform.localPosition.z - _defaultPosition.z;
    }
}
