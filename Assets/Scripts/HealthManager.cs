using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private float _health = 100f;
    [SerializeField]
    private GameObject _deathParticles;
    [SerializeField]
    private Material _deathMaterial;

    private GameObject _deathParticleRef = null;
    
    private bool isDead;

    void OnParticleCollision(GameObject other) {
        if (other.name == "M61 Vulcan") _health -= 20;
    }

    void DetachParticles() {
        if(_deathParticleRef != null) {
            var particleEmission = _deathParticleRef.GetComponent<ParticleSystem>().emission;
            particleEmission.enabled = false;
            _deathParticleRef.transform.parent = null;
            Destroy(_deathParticleRef.transform.gameObject, 5.0f);
        }
    }

    void Update()
    {
        if (!isDead && _health <= 0)
        {
            isDead = true;
            DeActivateAircraft();
            EnactDeathVisuals();
            Invoke("DetachParticles", 19.9f);
            Destroy(gameObject, 20.0f);
        }
    }

    void EnactDeathVisuals()
    {
        if (_deathMaterial != null)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                Material[] deathMaterials = new Material[renderer.materials.Length];
                for (int j = 0; j < deathMaterials.Length; ++j) {
                    deathMaterials[j] = _deathMaterial;
                }
                renderer.materials = deathMaterials;
            }
        }

        if (_deathParticles != null)
        {
            _deathParticleRef = Instantiate(_deathParticles, transform);
        }
    }

    void DeActivateAircraft()
    {
        AerodynamicController aeroController = GetComponent<AerodynamicController>();
        if (aeroController != null)
        {
            aeroController.SetEngineThrust(0f);
            
            AIPilot autoPilot = GetComponentInChildren<AIPilot>();
            if (autoPilot != null)
            {
                autoPilot.IsEnabled = false;
            }
        }
    }

    public void AddToHealth(float hp)
    {
        _health += hp;
    }

    public float GetHealth()
    {
        return _health;
    }
}
