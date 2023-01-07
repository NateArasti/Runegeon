using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyPack : ScriptableObject
{
    [SerializeField] private EnemyData[] m_Enemies;

    public IReadOnlyList<EnemyData> Enemies => m_Enemies;
}
