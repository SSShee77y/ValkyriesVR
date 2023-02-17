using UnityEngine;
using UnityEngine.Events;

public class ObjectiveEvents : MonoBehaviour
{
    [Header("Runs Only with an Objective Script")]
    [SerializeField]
    private UnityEvent _onObjectiveEnable;
    [SerializeField]
    private UnityEvent _onObjectiveComplete;

    public void InvokeEnableObjEvent()
    {
        _onObjectiveEnable.Invoke();
    }

    public void InvokeCompleteObjEvent()
    {
        _onObjectiveComplete.Invoke();
    }
}
