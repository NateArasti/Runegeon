using UnityEngine;

public interface IPatroller
{
    public bool HasReachedPatrolDestination();

    public void SetNextTarget(bool forceSet = false);
}
