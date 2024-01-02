using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPilot : MonoBehaviour
{
    [SerializeField]
    public bool IsEnabled;
    
    [Header("Navigational Variables")]
    // private serailize variables
    [SerializeField]
    private FollowPath _flightPath;
    [SerializeField]
    private Transform _navPoint;
    [SerializeField]
    private float _withinDistance = 50f;
    [SerializeField]
    private AerodynamicController _aeroController;
    [SerializeField] [Tooltip("The max angle factor | higher value means higher sensitivity to angles")]
    private float _maxRollAngle = 30f;
    [SerializeField] [Tooltip("The clamp on the max angle factor")]
    private float _safeRollAngle = 10f;
    [SerializeField] [Tooltip("The max angle factor | higher value means higher sensitivity to angles")]
    private float _maxPitchAngle = 30f;
    [SerializeField] [Tooltip("The clamp on the max angle factor")]
    private float _safePitchAngle = 10f;
    [SerializeField]
    private float _neededRollAngleForPitch = 10f;
    [SerializeField]
    private float _rollUprightAngle = 3f;

    // private variables
    private List<AerodynamicController.ControlSurface> _controlSurfaces = new List<AerodynamicController.ControlSurface>();

    [Header("Combat Variables")]
    // private serialize variables
    [SerializeField]
    public bool engageCombat = true;
    [SerializeField]
    private float _engagementDistance = 10000f;
    [SerializeField]
    private float _missileCooldown = 2f;
    [SerializeField]
    private float _missileFireAngle = 15f;
    [SerializeField]
    private float _gunFireAngle = 5f;
    [SerializeField]
    private float _missileEvasionDistance = 2000f;
    
    // private variables
    private PlayerWeaponry _weaponsControl;
    private Transform _currentMissile;
    private Transform _missileAtMe;
    private float _missileShootTimer;
    private float _randomEvadeValue;
    
    void OnDrawGizmos()
    {
        if (IsEnabled)
        {
            if (_navPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _navPoint.position);
                if (_navPoint.GetComponent<Rigidbody>() != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, PredictedTargetPosition(_navPoint));
                }
            }
            else if (_flightPath != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _flightPath.GetFirstPoint().position);
            }
        }
    }

    void Start()
    {
        if (_flightPath != null)
        {
            if (_flightPath.Contains(_navPoint) == false)
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

        _weaponsControl = _aeroController.GetComponent<PlayerWeaponry>();
    }

    void Update()
    {
        if (IsEnabled)
        {
            CheckPosition();
            RotatePlaneToLocation();
            FireControl();
        }
        else
        {
            foreach (AerodynamicController.ControlSurface cs in _controlSurfaces)
            {
                if (cs.transform.GetComponent<RotationTransformConstrainer>() != null)
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

        if (Evasion() == true)
        {
            rollAmount = _randomEvadeValue;
            pitchAmount = 100f;
        }

        else if (_navPoint != null)
        {
            Vector3 targetPosition = (_navPoint.GetComponent<Rigidbody>() != null) ? PredictedTargetPosition(_navPoint) : _navPoint.position;
        
            // relativeDisplacement = local space displacement vector to target
            // relativeVelocity = local space velocity vector of the place
            Vector3 relativeDisplacement = GetRelativeDisplacement(targetPosition - transform.position);
            Vector3 relativeVelocity = GetRelativeDisplacement(_aeroController.GetComponent<Rigidbody>().velocity);

            float relativeHorizontalAngle = Mathf.Atan((relativeDisplacement.x - relativeVelocity.x) / relativeVelocity.z);

            float relativeVerticleAngle = Vector3.Angle(relativeDisplacement.normalized, relativeVelocity.normalized) * ((relativeDisplacement.y < relativeVelocity.y) ? -1f : 1f);

            if (relativeDisplacement.z < 0)
                pitchAmount = 1f;
            else
                pitchAmount = (Mathf.Abs(relativeHorizontalAngle) < _neededRollAngleForPitch * Mathf.Deg2Rad) ? ((relativeVerticleAngle < 90) ? Mathf.Sin(relativeVerticleAngle * Mathf.Deg2Rad) : 1f) : 0f;

            rollAmount = Mathf.Sin(relativeHorizontalAngle);

            if (Mathf.Abs(relativeVerticleAngle) < _rollUprightAngle)
            {
                float localZRadian = Mathf.Deg2Rad * transform.localEulerAngles.z;
                float factor = Vector3.Dot(transform.right, Vector3.up);
                float signage = Mathf.Sign(Mathf.Sin(localZRadian));
                float percentage = (_rollUprightAngle - relativeVerticleAngle) / _rollUprightAngle;
                rollAmount *= 1 + (1 - percentage);
                rollAmount += factor * signage * percentage;
                rollAmount /= 2;
            }

        }

        foreach (AerodynamicController.ControlSurface cs in _controlSurfaces)
        {
            if (cs.transform.GetComponent<RotationTransformConstrainer>() != null)
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
            {   float pitchAngle = Mathf.Clamp(pitchAmount * _maxPitchAngle, -_safePitchAngle, _safePitchAngle);
                cs.transform.localEulerAngles = new Vector3(pitchAngle, cs.transform.localEulerAngles.y, cs.transform.localEulerAngles.z);
            }
        }
    }

    void FireControl()
    {
        if (engageCombat == false)
        {
            return;
        }

        // Take target from weapons control radar
        if (_navPoint == null)
        {
            _missileShootTimer = _missileCooldown;
            if (_weaponsControl.EnemiesList.Count > 0)
            {
                int randomIndex = Random.Range(0, _weaponsControl.EnemiesList.Count);
                _weaponsControl.SetNextEnemy(randomIndex);
                _navPoint = _weaponsControl.EnemiesList[randomIndex].gameObject.transform;
            }
            return;
        }

        // If target is dead, remove target from nav
        HealthManager targetHealth = _navPoint.GetComponent<HealthManager>();
        if (targetHealth != null && targetHealth.IsDead)
        {
            _navPoint = null;
            _weaponsControl.FireWeapon(false, 0);
            return;
        }

        // Get distance to target and attack angle to target
        float distanceToTarget = Vector3.Distance(_navPoint.transform.position, transform.position);
        Vector3 targetPosition = (_navPoint.GetComponent<Rigidbody>() != null) ? PredictedTargetPosition(_navPoint) : _navPoint.position;
        float angleBetween = Vector3.Angle(transform.forward, (targetPosition - transform.position).normalized);

        // If no target targeted,
        if (distanceToTarget >= _engagementDistance || angleBetween >= _missileFireAngle)
        {
            if (_missileShootTimer < _missileCooldown)
                _missileShootTimer += Time.deltaTime;
            else
                _missileShootTimer = _missileCooldown;
            return;
        }

        // GUNS GUNS GUNS
        if (angleBetween < _gunFireAngle && distanceToTarget < 1850 && distanceToTarget > 150)
        {
            _weaponsControl.FireWeapon(true, 0);
        }
        else
        {
            _weaponsControl.FireWeapon(false, 0);
        }
        
        // If missile has not been fired, fire it
        if (_currentMissile == null && _missileShootTimer <= 0)
        {
            _missileShootTimer = _missileCooldown;
            _currentMissile = _weaponsControl.FireWeapon(true, 1);
            return;
        }
        else if (_currentMissile == null && _missileShootTimer > 0)
        {
            _missileShootTimer -= Time.deltaTime;
        }
        
    }

    public bool Evasion()
    {
        if (_missileAtMe != null && _missileAtMe.GetComponent<MissileController>().Target == _aeroController.gameObject)
        {
            return true;
        }

        foreach (MissileController missile in FindObjectsOfType<MissileController>())
        {
            if (missile.Activated && missile.Target == _aeroController.gameObject
                    && Vector3.Distance(missile.transform.position, transform.position) < _missileEvasionDistance)
            {
                _missileAtMe = missile.transform;
                _randomEvadeValue = Random.Range(-1f, 1f);
                return true;
            }
        }

        return false;
    }

    Vector3 GetRelativeDisplacement(Vector3 displacement)
    {
        Vector3 relativeDisplacement = new Vector3();
        relativeDisplacement.z = Vector3.Dot(displacement, transform.forward);
        relativeDisplacement.y = Vector3.Dot(displacement, transform.up);
        relativeDisplacement.x = Vector3.Dot(displacement, transform.right);

        return relativeDisplacement;
    }

    Vector3 PredictedTargetPosition(Transform target)
    {
        float velocityMagnitude = _aeroController.GetComponent<Rigidbody>().velocity.magnitude;

        float timeFromTarget = Mathf.Min(2f, Vector3.Distance(transform.position, target.position) / velocityMagnitude);

        Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity;

        Vector3 predictedPosition = target.position + (timeFromTarget * targetVelocity);

        for (int i = 0; i < 5; i++)
        {
            timeFromTarget = Mathf.Min(1.5f, Vector3.Distance(transform.position, target.position) / velocityMagnitude);
            predictedPosition = target.position + (timeFromTarget * targetVelocity);
        }

        return predictedPosition;
    }
}
