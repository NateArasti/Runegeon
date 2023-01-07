using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupSwitch : MonoBehaviour
{
    [SerializeField] private bool m_DisableOnStart = true;
    [SerializeField] private bool m_Animate = true;
    [SerializeField, ShowIf(nameof(m_Animate))] private float m_AnimationDuration = 1;
    private CanvasGroup m_CanvasGroup;

    private void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (m_DisableOnStart) Switch(false);
    }

    public void Switch(bool enabled)
    {
        m_CanvasGroup.DOKill();
        m_CanvasGroup.blocksRaycasts = enabled;
        m_CanvasGroup.interactable = enabled;

        if (m_Animate)
        {
            m_CanvasGroup.DOFade(enabled ? 1 : 0, m_AnimationDuration);
        }
        else
        {
            m_CanvasGroup.alpha = enabled ? 1 : 0;
        }
    }
}
