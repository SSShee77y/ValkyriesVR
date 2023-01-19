using System.Collections.Generic;
using UnityEngine;

public class MissileController : Aerodynamics
{
    [Header("Missile Specific Settings")]
    [SerializeField]
    private float _turnFactor = 10f;
    [SerializeField]
    private float _fuelTime = 2.5f;
    [SerializeField]
    private float _lifeTime = 60f;
    [SerializeField]
    private int _missileAccuracy = 4;
    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private GameObject _explosion;

    private bool _activated;
    private ParticleSystem _particles;
    private Vector3 _targetPreviousVelocity;
    private AudioSource _launchAudio;
    
    void OnCollisionEnter(Collision other)
    {
        _rb.velocity = new Vector3();
        DetachParticles();
        if (_activated == true)
        {
            Instantiate(_explosion, other.contacts[0].point, this.transform.rotation);
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
        if (_activated == true)
        {
            gameObject.transform.parent = null;
            _rb.constraints = RigidbodyConstraints.None;
            if (_target == null) _lifeTime -= Time.fixedDeltaTime;

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
        if (_lifeTime <= 0) {
            DetachParticles();
            Destroy(this.gameObject);
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
        if (_target != null) RotateTowardsTarget(dragAmount);
    }

    void RotateTowardsTarget(float dragAmount)
    {
        Vector3 neededVelocity = RequiredVelocity();

        Vector3 velocityPosition = new Vector3();

        velocityPosition = (neededVelocity - _rb.velocity);

        Vector3 newDirection = Vector3.Normalize(velocityPosition);

        Vector3 positionDifference = (PredictedTargetPosition() - transform.position);

        if (Vector3.Angle(new Vector3(positionDifference.normalized.x, 0, positionDifference.normalized.z), new Vector3(transform.forward.x, 0, transform.forward.z)) <= 15)
        {
            if (Mathf.Abs(positionDifference.x) < Mathf.Abs(positionDifference.z))
                newDirection.z *= 0;
            else if (Mathf.Abs(positionDifference.x) > Mathf.Abs(positionDifference.z))
                newDirection.x *= 0;
        } else {
            newDirection = positionDifference.normalized;
        }
            
        float turnMultiplier = TurnMultiplier(neededVelocity.normalized);

        Quaternion rotation = Quaternion.LookRotation(newDirection);

        transform.localRotation = Quaternion.Slerp(Quaternion.LookRotation(transform.forward), rotation, turnMultiplier * _turnFactor * Time.fixedDeltaTime);
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

    Vector3 RequiredVelocity()
    {
        Vector3 predictedPosition = PredictedTargetPosition();
        
        float timeFromTarget = Vector3.Distance(transform.position, predictedPosition) / _rb.velocity.magnitude;
        
        Vector3 requiredVelocity = (predictedPosition - transform.position) / timeFromTarget;

        return requiredVelocity;
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
        Rigidbody parentRigidbody = new Rigidbody();
        while (currentObject.transform.parent.gameObject != null)
        {
            currentObject = currentObject.transform.parent.gameObject;
            parentRigidbody = currentObject.GetComponent<Rigidbody>();
            if (parentRigidbody != null)
            {
                gameObject.transform.parent = null;
                _rb.constraints = RigidbodyConstraints.None;
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
