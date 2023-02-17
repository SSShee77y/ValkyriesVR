using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeSystem : MonoBehaviour
{
    [SerializeField]
    private Transform _airbrake;
    [SerializeField]
    private Vector3 _brakeRotation;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private AerodynamicController _planeController;
    
    private ColliderController _cc;
    private Quaternion _normalRotation;
    private bool _brakeActivated;

    void Start()
    {
        _normalRotation = _airbrake.localRotation;
        _cc = _planeController.GetComponent<ColliderController>();        
    }

    void Update()
    {
        Quaternion toRotation = _airbrake.localRotation;

        if (_brakeActivated)
        {
            toRotation = Quaternion.Euler(_brakeRotation);
            SetWheelBrakeTorque(10_000f);
            _cc.SetIsBraking(true);
        }
        else
        {
            toRotation = _normalRotation;
            SetWheelBrakeTorque(0f);
            _cc.SetIsBraking(false);
        }

        _airbrake.localRotation = Quaternion.RotateTowards(_airbrake.localRotation, toRotation, _rotationSpeed * Time.deltaTime);
    }

    void SetWheelBrakeTorque(float brakeAmount)
    {
        foreach (WheelCollider w in _planeController.GetComponentsInChildren<WheelCollider>())
        {
            w.brakeTorque = brakeAmount;
        }
    }

    [ContextMenu("ToggleBrakeState")]
    void ToggleBrakeState()
    {
        _brakeActivated = !_brakeActivated;
    }

    public void SetBrakeActive(bool isActive)
    {
        _brakeActivated = isActive;
    }
}
