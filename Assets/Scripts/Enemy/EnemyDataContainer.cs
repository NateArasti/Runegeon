using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDataContainer : MonoBehaviour
{
    public void Set(IReadOnlyList<Transform> patrolPoints)
    {
        if(TryGetComponent<Patroller>(out var patroller))
        {
            patroller.Targets = patrolPoints.ToArray();
        }
    }
}