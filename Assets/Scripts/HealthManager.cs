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
        if (other.name == "M61 Vulcan") _health -= 5;
    }

    void Update()
    {
        if (!isDead && _health <= 0)
        {
            isDead = true;
            if (_deathMaterial != null)
            {
                foreach (var renderer in GetComponentsInChildren<Renderer>())
                {
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        renderer.materials[i] = _deathMaterial;
                    }
                }
            }

            if (_deathParticles != null)
            {
                Instantiate(_deathParticles, transform.position, transform.rotation);
            }

            gameObject.tag = "Untagged";
            Destroy(gameObject, 10);
        }
    }

    public void AddToHealth(float hp)
    {
        _health += hp;
    }
}
