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
    private Rigidbody _planeRigidbody;

    [SerializeField]
    private ColliderController _referenceColliderController;

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
    private bool invokedEventOne = false;

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
        wheelsGrounded = _referenceColliderController.AnyWheelIsColldingWithGround();
    }

    void ModifyConstrainer()
    {
        if (wheelsGrounded) _plc.ZPosition.MaxDisplacment = -0.004f;
        else _plc.ZPosition.MaxDisplacment = 0.018f;
    }

    void CheckPositions()
    {
        foreach (RotationSurface surface in _surfaceList)
        {
            Quaternion toRotation = surface.transform.localRotation;

            if (transform.localPosition.Equals(_firstPosition))
            {
                toRotation = Quaternion.Euler(surface.rotationOne);
            }
            else if (transform.localPosition.Equals(_secondPosition))
            {
                toRotation = Quaternion.Euler(surface.rotationTwo);
            }
            
            surface.transform.localRotation = Quaternion.RotateTowards(surface.transform.localRotation, toRotation, surface.rotationSpeed * Time.deltaTime);
        }

        if (transform.localPosition.Equals(_firstPosition) && invokedEventOne == false)
        {
            Vector3 velocity = _planeRigidbody.velocity;
            Vector3 angularVelocity = _planeRigidbody.angularVelocity;
            _gearEvent.eventOne.Invoke();
            invokedEventOne = true;
            _planeRigidbody.velocity = velocity;
            _planeRigidbody.angularVelocity = angularVelocity;
        }
        else if (transform.localPosition.Equals(_secondPosition) && invokedEventOne == true)
        {
            _gearEvent.eventTwo.Invoke();
            invokedEventOne = false;
        }
    }
}
