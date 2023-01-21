using UnityEngine;

public class GoToLocation : Objective
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float withinRange;

    void Update()
    {

    }

    protected override void FinishObjective()
    {
        objectiveFinished = true;
    }

    bool IsWithinRange()
    {
        return (Vector3.Distance(player.position, transform.position) <= Mathf.Abs(withinRange));
    }
}
