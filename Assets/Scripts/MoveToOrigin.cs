using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToOrigin : MonoBehaviour
{
    [SerializeField]
    private float moveRange = 1000f;

    private GameObject _player;

    private ParticleSystem.Particle[] _particles = null;
    
    [HideInInspector]
    public float HeightDisplacement = 0;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Vector3 movePositions = CheckPlayerPosition();
        if (!movePositions.Equals(new Vector3()))
        {
            HeightDisplacement -= movePositions.y;
            MoveAllObjects(movePositions);
            MoveAllParticles(movePositions);
        }
    }

    Vector3 CheckPlayerPosition()
    {
        Vector3 movePosition = new Vector3();
        Vector3 playerPosition = _player.transform.position;
        
        if (playerPosition.x > moveRange) {
            movePosition.x = -moveRange;
        } else if (playerPosition.x < -moveRange) {
            movePosition.x = moveRange;
        }

        if (playerPosition.y > moveRange) {
            movePosition.y = -moveRange;
        } else if (playerPosition.y < -moveRange) {
            movePosition.y = moveRange;
        }

        if (playerPosition.z > moveRange) {
            movePosition.z = -moveRange;
        } else if (playerPosition.z < -moveRange) {
            movePosition.z = moveRange;
        }

        return movePosition;
    }

    void MoveAllObjects(Vector3 offset)
    {
        foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.transform != null)
                go.transform.position += offset;
        }
    }

    void MoveAllParticles(Vector3 offset)
    {
        foreach (ParticleSystem ps in FindObjectsOfType<ParticleSystem>())
        {
            if (ps.main.simulationSpace != ParticleSystemSimulationSpace.World)
                continue;
 
            int particlesNeeded = ps.main.maxParticles;
 
            if (particlesNeeded <= 0)
                continue;

            // bool wasPaused = ps.isPaused;
            // bool wasPlaying = ps.isPlaying;

            // if (!wasPaused)
            //     ps.Pause();
 
            if (_particles == null || _particles.Length < particlesNeeded)
            {
                _particles = new ParticleSystem.Particle[particlesNeeded];
            }
 
            int num = ps.GetParticles(_particles);
 
            for (int i = 0; i < num; i++)
            {
                _particles[i].position += offset;
            }

            ps.SetParticles(_particles, num);

            // if (wasPlaying)
            //     ps.Play();
        }
    }
}
