using BehavioursRectangularGraph;
using UnityEngine;

public class GlobalPlayerData : MonoBehaviour
{
    private static GlobalPlayerData m_Instance;
    public static Vector2 PlayerPosition => m_Instance.m_Player.position;

    [SerializeField] private Transform m_Player;

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
    }

    public void SetPlayerToStartPosition(RectangularNode<Room> startRoom)
    {
        m_Player.position = startRoom.SpawnedBehaviour.StartPosition;
    }
}
