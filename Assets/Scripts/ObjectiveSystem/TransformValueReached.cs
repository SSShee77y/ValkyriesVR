using UnityEngine;

public class TransformValueReached : ValueReached
{
    [SerializeField]
    private new Transform objectiveComponent;
    [SerializeField]
    private bool useRotationInstead;
    [SerializeField]
    private Vector3 minVectorValue;
    [SerializeField]
    private Vector3 maxVectorValue;

    void Start()
    {
        base.objectiveComponent = objectiveComponent;
    }

    void Update()
    {
        if (IsValueReached())
            FinishObjective();
    }

    protected override void FinishObjective()
    {
        _objectiveFinished = true;
    }

    protected override bool IsValueReached()
    {
        Vector3 vectorValue = objectiveComponent.position;
        if (useRotationInstead)
        {
            vectorValue = objectiveComponent.eulerAngles;
        }
        if (vectorValue.x > minVectorValue.x && vectorValue.x < maxVectorValue.x &&
            vectorValue.y > minVectorValue.y && vectorValue.y < maxVectorValue.y &&
            vectorValue.z > minVectorValue.z && vectorValue.z < maxVectorValue.z)
            return true;
        return false;
    }
}
