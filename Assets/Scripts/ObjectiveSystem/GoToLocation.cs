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

    [SerializeField]
    private GameObject _waypointRing;

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

    protected override void Start()
    {
        base.Start();
        
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

        if (_waypointRing != null) UpdateWaypointRing();
    }

    void UpdateWaypointRing()
    {
        if (_navPoint == null)
        {
            _waypointRing.SetActive(false);
            return;
        }
        _waypointRing.SetActive(true);
        _waypointRing.transform.position = _navPoint.position;
        Vector3 relativePos = _player.position - _waypointRing.transform.position;
        _waypointRing.transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
    }

    public override void FinishObjective()
    {
        base.FinishObjective();
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
