using UnityEngine;

public class GoToLocation : Objective
{
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private FollowPath _flightPath;
    [SerializeField]
    private Transform _navPoint;
    [SerializeField]
    private float _withinRange = 50f;

    private int _currentPoints = 0;
    private int _totalPointsNeeded = 0;

    void OnDrawGizmos()
    {
        if (_navPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _navPoint.position);
        }
        else if (_flightPath != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _flightPath.GetFirstPoint().position);
        }
    }

    void Start()
    {
        if (_flightPath != null)
        {
            if (_flightPath.Contains(_navPoint) == false)
                _navPoint = _flightPath.GetFirstPoint();
        }

        if (_flightPath != null)
        {
            _totalPointsNeeded = _flightPath.transform.childCount;
        }
        else if (_navPoint != null)
        {
            _totalPointsNeeded = 1;
        }
    }

    void Update()
    {
        if (IsWithinRange())
        {
            _currentPoints++;
        }

        if (_currentPoints >= _totalPointsNeeded)
        {
            FinishObjective();
        }
    }

    protected override void FinishObjective()
    {
        _objectiveFinished = true;
        if (_nextObjective != null)
            _nextObjective.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    bool IsWithinRange()
    {
        if (_navPoint != null && Vector3.Distance(_navPoint.position, _player.transform.position) <= _withinRange)
        {
            if (_flightPath != null)
            {
                _navPoint = _flightPath.GetNextPoint(_navPoint);
            }
            else
            {
                _navPoint = null;
            }
            return true;
        }
        return false;
    }
}
