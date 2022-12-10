using UnityEngine;

public interface IPatroller
{
    public bool HasReachedDestination();

    public void SetNextTarget(bool forceSet = false);
}
