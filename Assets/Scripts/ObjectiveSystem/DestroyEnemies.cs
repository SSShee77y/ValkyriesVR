using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemies : Objective
{
    [SerializeField]
    private List<GameObject> _enemies = new List<GameObject>();
    [SerializeField]
    private int _enemiesToRemain = 0;

    private int _startingCount;

    protected override void Start()
    {
        base.Start();
        _startingCount = _enemies.Count;
    }

    void Update()
    {
        CheckEnemies();
        if (_enemies.Count <= _enemiesToRemain)
            FinishObjective();
    }

    void CheckEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].GetComponent<HealthManager>().GetHealth() <= 0 || _enemies[i] == null)
            {
                _enemies.RemoveAt(i);
                i--;
            }
        }
    }

    public override void FinishObjective()
    {
        base.FinishObjective();
    }

    public override string GetDescription()
    {
        return string.Format("{0}\n\n({1}) / ({2}) enemies destroyed", _description, _startingCount - _enemies.Count, _startingCount - _enemiesToRemain);
    }

}
