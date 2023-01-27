using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    private Objective _currentObjective;

    void Start()
    {
        _currentObjective = GetComponentInChildren<Objective>();
    }

    void Update()
    {
        if (!_currentObjective.gameObject.activeInHierarchy)
        {
            _currentObjective = GetComponentInChildren<Objective>();
        }
    }

    [ContextMenu("FinishCurrentObjective")]
    public void FinishCurrentObjective()
    {
        _currentObjective.FinishObjective();
        _currentObjective = GetComponentInChildren<Objective>();
    }

    public Objective GetCurrentObjective()
    {
        return _currentObjective;
    }
}
