using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HitEffect : MonoBehaviour
{
    [SerializeField] private float m_EffectDuration = 0.2f;
    private SpriteRenderer m_SpriteRenderer;

    private readonly int HitProperty = Shader.PropertyToID("_HitApplier");

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    [Button]
    public void Play()
    {
        m_SpriteRenderer.material.DOFloat(1, HitProperty, 0.5f * m_EffectDuration)
            .OnComplete(() => m_SpriteRenderer.material.DOFloat(0, HitProperty, 0.5f * m_EffectDuration));
    }
}
