using System.Collections.Generic;
using UnityEngine;

public class MoveToOrigin : MonoBehaviour
{
    [SerializeField]
    private float moveRange = 1000f;

    private GameObject _player;
    private List<GameObject> _gameObjects = new List<GameObject>();
    private int nonMissileCount;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _gameObjects.Add(_player);
        _gameObjects.AddRange(GameObjectNoParentWithTag("Ally"));
        _gameObjects.AddRange(GameObjectNoParentWithTag("Enemy"));
        _gameObjects.AddRange(GameObjectNoParentWithTag("Terrain"));
        nonMissileCount = _gameObjects.Count;
        
        _gameObjects.AddRange(GameObjectNoParentWithTag("Missile"));
    }

    void Update()
    {
        /*
         *  Add particles to move as well
         */
        ReAddMissiles();
        Vector3 movePositions = CheckPlayerPosition();
        if (!movePositions.Equals(new Vector3()))
        {
            MoveAllObjects(movePositions);
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

    void ReAddMissiles()
    {
        while (_gameObjects.Count > nonMissileCount)
        {
            //_gameObjects.RemoveAt(_gameObjects.Count - 1);
        }
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

    void MoveAllObjects(Vector3 addPosition)
    {
        foreach (GameObject go in _gameObjects)
        {
            if (go.transform != null)
                go.transform.position += addPosition;
        }
    }
}
