using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    [SerializeField]
    protected string _title;
    
    [SerializeField] [TextArea]
    protected string _description;

    [SerializeField]
    protected Objective _nextObjective;
    
    protected bool _objectiveFinished;

    protected abstract void FinishObjective();

}
