using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    [SerializeField]
    protected string title;
    
    [SerializeField] [TextArea]
    protected string description;

    [SerializeField]
    protected Objective nextObjective;
    
    protected bool objectiveFinished;

    protected abstract void FinishObjective();

}
