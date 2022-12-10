using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : HealthSystem
{
    [Space]
    [SerializeField] private Slider m_HealthSlider;
    [SerializeField] private float m_HealthBarChangeDuration = 0.5f;

    protected override void OnHealthChanged()
    {
        m_HealthSlider.DOValue(CurrentHealth / MaxHealth, m_HealthBarChangeDuration);
    }
}
