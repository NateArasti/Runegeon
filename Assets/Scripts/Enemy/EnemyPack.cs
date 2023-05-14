using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyPack : ScriptableObject
{
    [SerializeField] private EnemyDataContainer[] m_Enemies;

    public IReadOnlyList<EnemyDataContainer> Enemies => m_Enemies;
}
