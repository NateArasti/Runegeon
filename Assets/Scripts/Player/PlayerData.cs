using BehavioursRectangularGraph;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private static PlayerData s_Instance;

    [SerializeField] private Transform m_PlayerCharacter;

    public static Transform PlayerTransform => s_Instance.m_PlayerCharacter;

    private void Awake()
    {
        s_Instance = this;
    }

    public void SetPlayerToStartPosition(RectangularNode<Room> startRoom)
    {
        transform.position = startRoom.SpawnedBehaviour.StartPosition;
    }
}
