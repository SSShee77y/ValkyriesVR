using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GearController : MonoBehaviour
{
    [Serializable]
    public class RotationSurface {
        public Transform transform;

        public Vector3 rotationOne;

        public Vector3 rotationTwo;

        public float rotationSpeed;
    }

    [Serializable]
    public class GearEvent {
        public UnityEvent eventOne;

        [SerializeField]
        public UnityEvent eventTwo;
    }

    [SerializeField]
    private WheelController _referenceWheelController;

    private bool wheelsGrounded;

    [SerializeField]
    private Vector3 _firstPosition;
    [SerializeField]
    private Vector3 _secondPosition;

    [SerializeField]
    private List<RotationSurface> _surfaceList;

    [SerializeField]
    private GearEvent _gearEvent;

    private PositionLockConstrainer _plc;

    void Start()
    {
        _plc = GetComponent<PositionLockConstrainer>();
    }

    void Update()
    {
        CheckWheels();
        ModifyConstrainer();
        if (!wheelsGrounded)
        {
            CheckPositions();
        }
    }

    void CheckWheels()
    {
        wheelsGrounded = _referenceWheelController.AnyWheelIsColldingWithGround();
    }

    void ModifyConstrainer()
    {
        if (wheelsGrounded) _plc.ZPosition.MaxDisplacment = -0.02f;
        else _plc.ZPosition.MaxDisplacment = 0.18f;
    }

    void CheckPositions()
    {
        foreach (RotationSurface surface in _surfaceList)
        {
            Quaternion toRotation = transform.localRotation;

            if (transform.localPosition.Equals(_firstPosition))
                toRotation = Quaternion.Euler(surface.rotationOne.x, surface.rotationOne.y, surface.rotationOne.z);
            else if (transform.localPosition.Equals(_secondPosition))
                toRotation = Quaternion.Euler(surface.rotationOne.x, surface.rotationOne.y, surface.rotationOne.z);

            if (!toRotation.Equals(transform.localRotation))
                surface.transform.localRotation = Quaternion.RotateTowards(transform.localRotation, toRotation, surface.rotationSpeed * Time.deltaTime);
        }

        if (transform.localPosition.Equals(_firstPosition))
            _gearEvent.eventOne.Invoke();
        else if (transform.localPosition.Equals(_secondPosition))
            _gearEvent.eventTwo.Invoke();
    }
}
