using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveToUI : MonoBehaviour
{
    private ObjectiveManager _objectiveManager;
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _description;

    void Start()
    {
        _objectiveManager = FindObjectOfType<ObjectiveManager>();
    }

    void Update()
    {
        if (_objectiveManager != null) MatchTextToObjectives();
    }

    void MatchTextToObjectives()
    {
        Objective currentObjective = _objectiveManager.GetCurrentObjective();
        _title.text = currentObjective.GetTitle();
        _description.text = currentObjective.GetDescription();
    }
}
