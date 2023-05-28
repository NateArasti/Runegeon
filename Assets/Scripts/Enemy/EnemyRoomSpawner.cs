using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

[RequireComponent(typeof(Room))]
public class EnemyRoomSpawner : MonoBehaviour
{
    [SerializeField] private EnemyPack m_EnemyPack;
    [SerializeField, MinMaxSlider(0, 10)] private Vector2Int m_EnemiesCountRange;
    [SerializeField] private Transform[] m_PatrolPoints;
    private Room m_Room;

    private void Awake()
    {
        m_Room = GetComponent<Room>();
    }

    private void Start()
    {
        if (m_EnemiesCountRange.y * 2 > m_PatrolPoints.Length)
        {
            Debug.LogError($"Not enough patrol point!!!\n " +
                $"Should be atleast twice as much as max enemies count but was {m_PatrolPoints.Length} point" +
                $" with {m_EnemiesCountRange.y} max enemies");
            return;
        }

        if (m_Room.Depth == 0) return;
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        var count = Random.Range(m_EnemiesCountRange.x, m_EnemiesCountRange.y + 1);
        var shuffledPatrolPoints = m_PatrolPoints.GetShuffled();
        var currentPatrolPointIndex = 0;

        for (int i = 0; i < count; i++)
        {
            var maxPointsCount = shuffledPatrolPoints.Count - currentPatrolPointIndex - (count - i - 2);
            var endPatrolPointIndex = currentPatrolPointIndex + Random.Range(2, maxPointsCount + 1) - 1;
            var patrolPoints = new List<Transform>();
            patrolPoints.AddRange(m_PatrolPoints[currentPatrolPointIndex..endPatrolPointIndex]);
            currentPatrolPointIndex = endPatrolPointIndex;

            Instantiate(
                m_EnemyPack.Enemies.GetRandomObject(), 
                patrolPoints[0].position, 
                Quaternion.identity, 
                transform)
                .Set(patrolPoints);
        }
    }

    [Button]
    private void GetAllPatrolPoints()
    {
        var patrolPoints = new List<Transform>();
        foreach (Transform point in transform.Find("PatrolPoints"))
        {
            patrolPoints.Add(point);
        }
        m_EnemiesCountRange = new Vector2Int(Mathf.Max(2, patrolPoints.Count / 2 - 2), patrolPoints.Count / 2);
        m_PatrolPoints = patrolPoints.ToArray();
    }
}
