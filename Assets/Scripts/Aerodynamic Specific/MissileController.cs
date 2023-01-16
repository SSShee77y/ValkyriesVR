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
    private Vector3 _previousVelocity;
    
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
        _previousVelocity = _rb.velocity;
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
        Quaternion rotation = Quaternion.LookRotation(DirectionToTarget());
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _turnFactor);
    }

    Vector3 DirectionToTarget()
    {
        Vector3 direction = (PredictedTargetPosition() - transform.position).normalized;
        Debug.Log(PredictedTargetPosition());
        return direction;
    }

    Vector3 PredictedTargetPosition()
    {
        
        Vector3 acceleration =
            new Vector3((_rb.velocity.x - _previousVelocity.x) / Time.fixedDeltaTime,
                        (_rb.velocity.y - _previousVelocity.y) / Time.fixedDeltaTime,
                        (_rb.velocity.z - _previousVelocity.z) / Time.fixedDeltaTime);

        float localForwardVelocity = Vector3.Dot(_rb.velocity, transform.forward);

        float timeFromTarget = Vector3.Distance(gameObject.transform.position, _target.transform.position) / localForwardVelocity;

        Vector3 predictedPosition =
            new Vector3(_target.transform.position.x,
                        _target.transform.position.y,
                        _target.transform.position.z);
        
        if (_target.GetComponent<Rigidbody>() == null)
        {
            return predictedPosition;
        }
        for (int i = 0; i < _missileAccuracy; i++)
        {
            timeFromTarget = Vector3.Distance(gameObject.transform.position, predictedPosition) / _rb.velocity.magnitude;

            predictedPosition =
                new Vector3(_target.transform.position.x + (timeFromTarget * _target.GetComponent<Rigidbody>().velocity.x),
                            _target.transform.position.y + (timeFromTarget * _target.GetComponent<Rigidbody>().velocity.y) + (timeFromTarget * -_rb.velocity.y),
                            _target.transform.position.z + (timeFromTarget * _target.GetComponent<Rigidbody>().velocity.z));
        }
        _previousVelocity = _rb.velocity;
        return predictedPosition;
    }

}
