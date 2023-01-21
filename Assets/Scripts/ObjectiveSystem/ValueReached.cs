using UnityEngine;

public abstract class ValueReached : Objective
{
    protected Component objectiveComponent;

    protected abstract bool IsValueReached();
}
