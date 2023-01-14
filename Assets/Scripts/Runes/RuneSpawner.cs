using BehavioursRectangularGraph;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class RuneSpawner : MonoBehaviour
{
    [SerializeField] private LocationGenerator m_Generator;
    [SerializeField] private Rune[] m_PossibleRunes;
    [SerializeField, MinMaxSlider(0, 10)] private Vector2Int m_SpawnRange;

    public void SpawnRunes()
    {
        var deadEnds = new List<RectangularNode<Room>>();
        foreach (var node in m_Generator.RoomNodes)
        {
            if(node.IsDeadEnd)
                deadEnds.Add(node);
        }

        var spawnRoom = deadEnds.GetRandomObject();

        Instantiate(
            m_PossibleRunes.GetRandomObject(), 
            spawnRoom.SpawnedBehaviour.StartPosition, 
            Quaternion.identity
            );        
    }
}
