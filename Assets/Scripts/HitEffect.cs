using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HitEffect : MonoBehaviour
{
    [SerializeField] private float m_EffectDuration = 0.2f;
    private Material m_Material;

    private readonly int HitProperty = Shader.PropertyToID("_HitApplier");

    private void Awake()
    {
        m_Material = GetComponent<SpriteRenderer>().material;
    }

    [Button]
    public void Play()
    {
        m_Material.DOFloat(1, HitProperty, 0.5f * m_EffectDuration)
            .OnComplete(() => m_Material.DOFloat(0, HitProperty, 0.5f * m_EffectDuration));
    }
}
