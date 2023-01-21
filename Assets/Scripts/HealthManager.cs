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
    
    private bool isDead;

    void OnParticleCollision(GameObject other) {
        if (other.name == "M61 Vulcan") _health -= 10;
    }

    void Update()
    {
        if (!isDead && _health <= 0)
        {
            isDead = true;
            if (_deathMaterial != null)
            {
                GetComponent<Renderer>().material = _deathMaterial;

                foreach (var childRenderer in GetComponentsInChildren<Renderer>())
                {
                    childRenderer.material = _deathMaterial;
                }
            }

            if (_deathParticles != null)
            {
                Instantiate(_deathParticles, transform.position, transform.rotation);
            }

            Destroy(gameObject, 20);
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
