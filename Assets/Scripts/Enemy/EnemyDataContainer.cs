using System.Collections.Generic;
using UnityEngine;

public class EnemyDataContainer : MonoBehaviour
{
    public IReadOnlyList<Transform> PatrolPoints { get; private set; }

    public void Set(IReadOnlyList<Transform> patrolPoints)
    {
        PatrolPoints = patrolPoints;
    }
}