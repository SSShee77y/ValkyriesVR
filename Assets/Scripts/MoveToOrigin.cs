using System.Collections.Generic;
using UnityEngine;

public class MoveToOrigin : MonoBehaviour
{
    [SerializeField]
    private float moveRange = 1000f;

    private GameObject _player;
    private List<GameObject> _gameObjects = new List<GameObject>();
    private int _nonTerrainCount;

    private ParticleSystem.Particle[] _particles = null;
    
    public float HeightDisplacement = 0;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _gameObjects.AddRange(GameObjectNoParentWithTag("Terrain"));
        _nonTerrainCount = _gameObjects.Count;

        _gameObjects.Add(_player);
        _gameObjects.AddRange(GameObjectNoParentWithTag("Ally"));
        _gameObjects.AddRange(GameObjectNoParentWithTag("Enemy"));
        _gameObjects.AddRange(GameObjectNoParentWithTag("Missile"));
    }

    void Update()
    {
        /*
         *  Add particles to move as well
         */
        ReAddObjects();
        Vector3 movePositions = CheckPlayerPosition();
        if (!movePositions.Equals(new Vector3()))
        {
            HeightDisplacement -= movePositions.y;
            MoveAllObjects(movePositions);
            MoveAllParticles(movePositions);
        }
    }

    List<GameObject> GameObjectNoParentWithTag(string tag)
    {
        List<GameObject> goList = new List<GameObject>();
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject go in gos)
        {
            if (go.transform.parent == null)
            {
                goList.Add(go);
            }
        }

        return goList;
    }

    void ReAddObjects()
    {
        while (_gameObjects.Count > _nonTerrainCount)
        {
            _gameObjects.RemoveAt(_gameObjects.Count - 1);
        }
        _gameObjects.Add(_player);
        _gameObjects.AddRange(GameObjectNoParentWithTag("Ally"));
        _gameObjects.AddRange(GameObjectNoParentWithTag("Enemy"));
        _gameObjects.AddRange(GameObjectNoParentWithTag("Missile"));
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
        foreach (GameObject go in _gameObjects)
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
