using System.Collections.Generic;
using UnityEngine;

public class MissileController : Aerodynamics
{
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
    private bool _activated;
    [SerializeField]
    private GameObject _explosion;

    private ParticleSystem _particles;
    
    void OnCollisionEnter(Collision other)
    {
        DetachParticles();
        Destroy(this.gameObject);
    }

    void OnDisable() {
        if(this.gameObject.scene.isLoaded) {
            if (_activated == true)
            {
                Instantiate(_explosion, this.transform.position, this.transform.rotation);
            }
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
    }

    protected override void FixedUpdate()
    {
        if (_activated == true)
        {
            gameObject.transform.parent = null;
            _rb.constraints = RigidbodyConstraints.None;
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
        RotateTowardsTarget(dragAmount);
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
        }
        
        Debug.Log(gameObject.name + " | " + newDirection + " | " + RequiredVelocity() + " | " + velocityPosition);
            
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
        float timeFromTarget = Vector3.Distance(gameObject.transform.position, _target.transform.position) / _rb.velocity.magnitude;

        Vector3 predictedPosition = _target.transform.position;
        
        if (_target.GetComponent<Rigidbody>() == null)
        {
            return predictedPosition;
        }
        for (int i = 0; i < _missileAccuracy; i++)
        {
            timeFromTarget = Vector3.Distance(gameObject.transform.position, predictedPosition) / _rb.velocity.magnitude;

            predictedPosition = _target.transform.position + (timeFromTarget * _target.GetComponent<Rigidbody>().velocity);
        }

        return predictedPosition;
    }

    Vector3 RequiredVelocity()
    {
        Vector3 predictedPosition = PredictedTargetPosition();
        
        float timeFromTarget = Vector3.Distance(gameObject.transform.position, predictedPosition) / _rb.velocity.magnitude;
        
        Vector3 requiredVelocity = (predictedPosition - transform.position) / timeFromTarget;

        return requiredVelocity;
    }
}
