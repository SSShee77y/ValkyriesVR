using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPilot : MonoBehaviour
{
    [SerializeField]
    private bool _isEnabled;
    [SerializeField]
    private FollowPath _flightPath;
    [SerializeField]
    private Transform _navPoint;
    [SerializeField]
    private float _withinDistance = 50f;
    [SerializeField]
    private AerodynamicController _aeroController;
    [SerializeField]
    private float _maxRollAngle = 30f;
    [SerializeField]
    private float _safeRollAngle = 10f;
    [SerializeField]
    private float _maxPitchAngle = 30f;
    [SerializeField]
    private float _safePitchAngle = 10f;
    [SerializeField]
    private float _neededRollAngleForPitch = 10f;
    
    private List<AerodynamicController.ControlSurface> _controlSurfaces = new List<AerodynamicController.ControlSurface>();
    
    void OnDrawGizmos()
    {
        if (_navPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _navPoint.position);
        }
        else if (_flightPath != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _flightPath.GetFirstPoint().position);
        }
        
    }

    void Start()
    {
        if (_flightPath != null)
        {
            _navPoint = _flightPath.GetFirstPoint();
        }

        List<AerodynamicController.ControlSurface.SurfaceType> neededSurfaceTypes = new List<AerodynamicController.ControlSurface.SurfaceType>();
        neededSurfaceTypes.Add(AerodynamicController.ControlSurface.SurfaceType.LeftAileron);
        neededSurfaceTypes.Add(AerodynamicController.ControlSurface.SurfaceType.RightAileron);
        neededSurfaceTypes.Add(AerodynamicController.ControlSurface.SurfaceType.Elevator);

        foreach (AerodynamicController.ControlSurface cs in _aeroController.ControlSurfaces)
        {
            if (neededSurfaceTypes.Contains(cs.surfaceType))
            {
                _controlSurfaces.Add(cs);
            }
        }
    }

    void Update()
    {
        if (_isEnabled)
        {
            CheckPosition();
            RotatePlaneToLocation();
        }
        else
        {
            foreach (AerodynamicController.ControlSurface cs in _controlSurfaces)
            {
                cs.transform.GetComponent<RotationTransformConstrainer>().SetActive(true);
            }
        }
        
    }

    void CheckPosition()
    {
        if (_navPoint != null && Vector3.Distance(_navPoint.position, transform.position) <= _withinDistance)
        {
            if (_flightPath != null)
            {
                _navPoint = _flightPath.GetNextPoint(_navPoint);
            }
            else
            {
                _navPoint = null;
            }
        }
    }

    void RotatePlaneToLocation()
    {
        float rollAmount = 0f, pitchAmount = 0f;

        if (_navPoint != null)
        {
            Vector3 relativeDisplacement = GetRelativeDisplacement(_navPoint.position - transform.position);
            Vector3 relativeVelocity = GetRelativeDisplacement(_aeroController.GetComponent<Rigidbody>().velocity);

            float relativeHorizontalAngle = Mathf.Atan((relativeDisplacement.x - relativeVelocity.x) / relativeVelocity.z);

            float relativeVerticleAngle = Vector3.Angle(relativeDisplacement.normalized, relativeVelocity.normalized);
            if (relativeDisplacement.y < relativeVelocity.y) relativeVerticleAngle *= -1f;

            rollAmount = Mathf.Sin(relativeHorizontalAngle);
            pitchAmount = (rollAmount < Mathf.Sin(_neededRollAngleForPitch * Mathf.Deg2Rad)) ? ((relativeVerticleAngle < 90) ? Mathf.Sin(relativeVerticleAngle * Mathf.Deg2Rad) : 1f) : 0f;
            if (relativeDisplacement.z < 0) pitchAmount = 1f;
        }

        foreach (AerodynamicController.ControlSurface cs in _controlSurfaces)
        {
            cs.transform.GetComponent<RotationTransformConstrainer>().SetActive(false);
            if (cs.surfaceType.Equals(AerodynamicController.ControlSurface.SurfaceType.LeftAileron))
            {
                float rollAngle = Mathf.Clamp(rollAmount * _maxRollAngle, -_safeRollAngle, _safeRollAngle);
                cs.transform.localEulerAngles = new Vector3(rollAngle, cs.transform.localEulerAngles.y, cs.transform.localEulerAngles.z);
            }
            else if (cs.surfaceType.Equals(AerodynamicController.ControlSurface.SurfaceType.RightAileron))
            {   float rollAngle = Mathf.Clamp(rollAmount * _maxRollAngle, -_safeRollAngle, _safeRollAngle);
                cs.transform.localEulerAngles = new Vector3(-rollAngle, cs.transform.localEulerAngles.y, cs.transform.localEulerAngles.z);
            }
            else if (cs.surfaceType.Equals(AerodynamicController.ControlSurface.SurfaceType.Elevator))
            {   float rollAngle = Mathf.Clamp(pitchAmount * _maxRollAngle, -_safeRollAngle, _safeRollAngle);
                cs.transform.localEulerAngles = new Vector3(rollAngle, cs.transform.localEulerAngles.y, cs.transform.localEulerAngles.z);
            }
        }
    }

    Vector3 GetRelativeDisplacement(Vector3 displacement)
    {
        Vector3 relativeDisplacement = new Vector3();
        relativeDisplacement.z = Vector3.Dot(displacement, transform.forward);
        relativeDisplacement.y = Vector3.Dot(displacement, transform.up);
        relativeDisplacement.x = Vector3.Dot(displacement, transform.right);

        return relativeDisplacement;
    }
}
