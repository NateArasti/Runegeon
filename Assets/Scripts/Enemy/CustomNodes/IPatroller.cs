using UnityEngine;

public interface IPatroller
{
    public bool HasReachedDestination(float remainingDistance = 0);

    public void SetNextTarget(bool forceSet = false);
}
