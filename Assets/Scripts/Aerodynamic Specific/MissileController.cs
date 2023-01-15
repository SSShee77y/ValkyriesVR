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
        
    }

}
