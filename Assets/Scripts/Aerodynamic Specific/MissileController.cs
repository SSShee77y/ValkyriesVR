using System.Collections.Generic;
using UnityEngine;

public class MissileController : Aerodynamics
{
    [Header("Missile Specific Settings")]
    [SerializeField]
    private float _maxTurnAngle = 5f;
    [SerializeField]
    private float _safeTurnAngle = 3f;
    [SerializeField]
    private float _timeBeforeFiring = 1f;
    [SerializeField]
    private float _trackingWaitTimeAfterFire = 0.5f;
    [SerializeField]
    private float _fuelTime = 2.5f;
    [SerializeField]
    private float _lifeTime = 10f;
    [SerializeField]
    private int _missileAccuracy = 4;
    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private GameObject _explosion;

    private bool _activated;
    private float _timeWhenActivated;
    private ParticleSystem _particles;
    private Vector3 _targetPreviousVelocity;
    private AudioSource _launchAudio;
    private float _lastDistanceFromObject;
    
    void OnDrawGizmos()
    {
        if (_activated == true && _target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + _rb.velocity);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, PredictedTargetPosition());
        }
    }

    void OnCollisionEnter(Collision other)
    {
        _rb.velocity = new Vector3();
        DetachParticles();
        if (_activated == true)
        {
            Instantiate(_explosion, other.contacts[0].point, this.transform.rotation);
            HealthManager hpManager = other.gameObject.GetComponent<HealthManager>();
            if (hpManager != null)
                hpManager.AddToHealth(-120f);
        }
        Destroy(this.gameObject);
    }

    void OnDisable() {
        if(this.gameObject.scene.isLoaded) {
            
        }
    }

    protected override void Start()
    {
        base.Start();

        ParticleSystem childParticles = GetComponentInChildren<ParticleSystem>();
        if (childParticles != null) {
            _particles = childParticles;
            ParticleEnabled(false);
        }

        if (GetComponent<AudioSource>() != null)
            _launchAudio = GetComponent<AudioSource>();
    }

    protected override void FixedUpdate()
    {
        if (_target != null)
        {
            _lifeTime = Mathf.Clamp(_lifeTime, -1f, 1.5f);
        }
        if (_activated == true && _timeBeforeFiring <= 0)
        {
            _rb.constraints = RigidbodyConstraints.None;
            _rb.useGravity = true;
            if (_target == null || (_target != null && _lastDistanceFromObject < Vector3.Distance(_target.transform.position, transform.position)))
                _lifeTime -= Time.fixedDeltaTime;

            ApplyDragLift();

            if (_fuelTime <= 0f)
            {
                ParticleEnabled(false);
            }
            else
            {   
                ApplyThrust();
                _fuelTime -= Time.fixedDeltaTime;
                ParticleEnabled(true);
            }
        }
        else
        {
            ParticleEnabled(false);
        }
        if (_activated == true && _timeBeforeFiring > -_trackingWaitTimeAfterFire)
        {
            _timeBeforeFiring -= Time.fixedDeltaTime;
        }
        if (_lifeTime <= 0)
        {
            DetachParticles();
            Destroy(this.gameObject);
        }
        if (_target != null)
        {
            _lastDistanceFromObject = Vector3.Distance(_target.transform.position, transform.position);
        }
    }

    void ParticleEnabled(bool isEnabled)
    {
        if (_particles != null)
        {
            var particleEmission = _particles.emission;
            particleEmission.enabled = isEnabled;
        }
    }

    void DetachParticles() {
        if(_particles != null) {
            ParticleEnabled(false);
            _particles.transform.parent = null;
            Destroy(_particles.transform.gameObject, 5.0f);
        }
    }

    protected override void AdditionalDragMethods(float dragAmount)
    {
        if (_target != null && _timeBeforeFiring <= -_trackingWaitTimeAfterFire) RotateTowardsTarget(dragAmount);
    }

    void RotateTowardsTarget(float dragAmount)
    {
        Vector3 predictedTargetPosition = PredictedTargetPosition();
        Vector3 relativeDisplacement = GetRelativeDisplacement(predictedTargetPosition - transform.position);
        Vector3 relativeVelocity = GetRelativeDisplacement(_rb.velocity);

        float relativeHorizontalAngle = Mathf.Atan((relativeDisplacement.x - relativeVelocity.x) / relativeVelocity.z);

        float relativeVerticleAngle = Mathf.Atan((relativeDisplacement.y - relativeVelocity.y) / relativeVelocity.z);

        float horizontalAmount = Mathf.Sin(relativeHorizontalAngle);
        float verticalAmount = Mathf.Sin(relativeVerticleAngle);
        if (relativeDisplacement.z < 0)
        {
            if (relativeVerticleAngle < 0)
                verticalAmount = -1f;
            else
                verticalAmount = 1f;
        }

        float yawAmount = Mathf.Clamp(horizontalAmount * _maxTurnAngle, -_safeTurnAngle, _safeTurnAngle) * (Mathf.Sqrt(relativeVelocity.z) / 25f);
        float pitchAmount = Mathf.Clamp(verticalAmount * _maxTurnAngle, -_safeTurnAngle, _safeTurnAngle) * (Mathf.Sqrt(relativeVelocity.z) / 25f);

        Debug.Log(horizontalAmount + " | " + verticalAmount);

        transform.eulerAngles += new Vector3(-pitchAmount, yawAmount, 0);
    }

    Vector3 GetRelativeDisplacement(Vector3 displacement)
    {
        Vector3 relativeDisplacement = new Vector3();
        relativeDisplacement.z = Vector3.Dot(displacement, transform.forward);
        relativeDisplacement.y = Vector3.Dot(displacement, transform.up);
        relativeDisplacement.x = Vector3.Dot(displacement, transform.right);

        return relativeDisplacement;
    }

    float TurnMultiplier(Vector3 direction)
    {
        float angleDifference = Vector3.Angle(direction, transform.forward);
        float multiplier = Mathf.Sin(angleDifference * Mathf.Deg2Rad) + 0.001f;
        return multiplier;
    }
    
    Vector3 PredictedTargetPosition()
    {
        float timeFromTarget = Vector3.Distance(transform.position, _target.transform.position) / _rb.velocity.magnitude;

        Vector3 predictedPosition = _target.transform.position;

        if (_target.GetComponent<Rigidbody>() != null)
        {
            Vector3 acceleration = new Vector3();
            if (_targetPreviousVelocity != null && _target.GetComponent<Rigidbody>() != null)
            {
                acceleration = (_target.GetComponent<Rigidbody>().velocity - _targetPreviousVelocity) / Time.fixedDeltaTime;
                _targetPreviousVelocity = _target.GetComponent<Rigidbody>().velocity;
            }
            for (int i = 0; i < _missileAccuracy; i++)
            {
                timeFromTarget = Vector3.Distance(transform.position, predictedPosition) / _rb.velocity.magnitude;

                predictedPosition = _target.transform.position + (timeFromTarget * _target.GetComponent<Rigidbody>().velocity) + (.5f * Mathf.Pow(timeFromTarget, 2) * acceleration);
            }
        }

        return predictedPosition;
    }

    [ContextMenu("ActivateMissile")]
    public void ActivateMissile()
    {
        SetMissileActive(true);
    }

    public void SetMissileActive(bool isActive)
    {
        _activated = isActive;
        RemoveFixedJoint();
        if (isActive && _launchAudio != null) _launchAudio.Play();
        GameObject currentObject = gameObject;
        while (currentObject.transform.parent != null)
        {
            currentObject = currentObject.transform.parent.gameObject;
            Rigidbody parentRigidbody = currentObject.GetComponent<Rigidbody>();
            if (parentRigidbody != null)
            {
                gameObject.transform.parent = null;
                _rb.constraints = RigidbodyConstraints.None;
                _rb.useGravity = true;
                _rb.velocity = parentRigidbody.velocity;
                return;
            }
        }
    }

    public void SetMissileActive(bool isActive, Vector3 withVelocity)
    {
        _activated = isActive;
        RemoveFixedJoint();
        if (isActive && _launchAudio != null) _launchAudio.Play();
        gameObject.transform.parent = null;
        _rb.constraints = RigidbodyConstraints.None;
        _rb.useGravity = true;
        _rb.velocity = withVelocity;
    }

    void RemoveFixedJoint()
    {
        if (GetComponent<FixedJoint>() != null)
        {
            Destroy(GetComponent<FixedJoint>());
        }
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }
    
}
