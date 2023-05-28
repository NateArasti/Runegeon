using NaughtyAttributes;
using UnityEngine;
using static BehavioursRectangularGraph.RectangularGraph<Room>;

public class GraphNodeBehaviourDataSerialized
{
    [SerializeField, Range(0, 1)] protected float m_SpawnChance = 1;
    [SerializeField, MinMaxSlider(0, 100)] protected Vector2Int m_SpawnRange = new Vector2Int(0, 10);

    public GraphNodeBehaviourDataSerialized(float spawnChance, Vector2Int spawnRange)
    {
        m_SpawnChance = spawnChance;
        m_SpawnRange = spawnRange;
    }
}

[System.Serializable]
public class CommonGraphNodeBehaviourDataSerialized : GraphNodeBehaviourDataSerialized
{
    [SerializeField] protected Room m_Room;

    public CommonGraphNodeBehaviourDataSerialized(Room room, float spawnChance, Vector2Int spawnRange) 
        : base(spawnChance, spawnRange)
    {
        m_Room = room;
    }

    public GraphNodeBehaviourData GraphNodeBehaviourData => 
        new(m_Room, m_SpawnChance, m_SpawnRange);
}

[System.Serializable]
public class SpecialGraphNodeBehaviourDataSerialized : GraphNodeBehaviourDataSerialized
{
    [SerializeField] protected Room[] m_RoomVariations;

    public SpecialGraphNodeBehaviourDataSerialized(Room[] roomVariations, float spawnChance, Vector2Int spawnRange)
        : base(spawnChance, spawnRange)
    {
        m_RoomVariations = roomVariations;
    }

    public SpecialGraphNodeBehaviourData GraphNodeBehaviourData =>
        new(m_RoomVariations, m_SpawnChance, m_SpawnRange);
}

[System.Serializable]
public class SequenceGraphNodeBehaviourDataSerialized : SpecialGraphNodeBehaviourDataSerialized
{
    [SerializeField] protected Room[] m_NextRoomVariations;

    public SequenceGraphNodeBehaviourDataSerialized(
        Room[] roomVariations, Room[] nextRoomVariations, float spawnChance, Vector2Int spawnRange)
        : base(roomVariations, spawnChance, spawnRange)
    {
        m_NextRoomVariations = nextRoomVariations;
    }

    public new SequenceGraphNodeBehaviourData GraphNodeBehaviourData =>
        new(m_NextRoomVariations, m_RoomVariations, m_SpawnChance, m_SpawnRange);
}
