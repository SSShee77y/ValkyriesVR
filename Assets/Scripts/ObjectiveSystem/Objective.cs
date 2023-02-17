using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField]
    protected string _title;
    
    [SerializeField] [TextArea]
    protected string _description;

    [SerializeField]
    protected Objective _nextObjective;
    
    protected bool _objectiveFinished;

    private ObjectiveEvents _objectiveEvents;

    protected virtual void Start()
    {
        _objectiveEvents = GetComponent<ObjectiveEvents>();
        if (_objectiveEvents != null) _objectiveEvents.InvokeEnableObjEvent();
    }

    public virtual void FinishObjective()
    {
        _objectiveFinished = true;
        if (_objectiveEvents != null) _objectiveEvents.InvokeCompleteObjEvent();
        
        if (_nextObjective != null)
            _nextObjective.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public virtual string GetTitle()
    {
        return _title;
    }

    public virtual string GetDescription()
    {
        return _description;
    }

}
