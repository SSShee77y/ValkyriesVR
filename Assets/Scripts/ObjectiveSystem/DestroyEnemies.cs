using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemies : Objective
{
    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();
    [SerializeField]
    private int enemiesToRemain = 0;

    void Update()
    {
        if (enemies.Count <= enemiesToRemain)
            FinishObjective();
    }

    protected override void FinishObjective()
    {
        _objectiveFinished = true;
    }

}
