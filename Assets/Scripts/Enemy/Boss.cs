using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private EnemyAI m_EnemySideKick;

    private List<EnemyAI> m_SpawnedEnemies = new();

    public void SpawnSideKick(bool start)
    {
        if (start)
        {
            var enemy = Instantiate(m_EnemySideKick, transform.position, Quaternion.identity, transform.parent);
            enemy.AlwaysSeePlayer = true;
            m_SpawnedEnemies.Add(enemy);
        }
    }

    private void OnDestroy()
    {
        foreach (var enemy in m_SpawnedEnemies)
        {
            if(enemy != null)
            {
                enemy.Die();
            }
        }
    }
}
