using BehavioursRectangularGraph;
using NaughtyAttributes;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private static PlayerData s_Instance;

    [SerializeField] private Transform m_PlayerCharacter;
    [SerializeField] private EffectApplier m_PlayerEffectApplier;
    [SerializeField] private bool m_AddEffectsFromStart;
    [SerializeField, ShowIf(nameof(m_AddEffectsFromStart))] private BaseRuneEffect[] m_EffectsFromStart;

    public static Transform PlayerTransform => s_Instance.m_PlayerCharacter;

    private void Awake()
    {
        s_Instance = this;
    }

    private void Start()
    {
        if (m_AddEffectsFromStart)
        {
            foreach (var effect in m_EffectsFromStart)
            {
                s_Instance.m_PlayerEffectApplier.AddRuneEffect(effect);
            }
        }
    }

    public void SetPlayerToStartPosition(RectangularNode<Room> startRoom)
    {
        transform.position = startRoom.SpawnedBehaviour.StartPosition;
    }

    public static void AddRuneEffect(IRuneEffect runeEffect)
    {
        s_Instance.m_PlayerEffectApplier.AddRuneEffect(runeEffect);
    }
}
