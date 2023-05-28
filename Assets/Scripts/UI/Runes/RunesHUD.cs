using UnityEngine;
using UnityEngine.UI;

public class RunesHUD : MonoBehaviour
{
    [SerializeField] private Image[] m_MiniRuneIcons;
    [SerializeField] private RunePanel[] m_RunePanels;
    [SerializeField] private GameObject m_StartDescription;

    public bool OpenWithStorage { get; set; }

    private void Start()
    {
        RunesContainer.OnChange += SetRunes;
        SetRunes();
    }

    private void OnDestroy()
    {
        RunesContainer.OnChange -= SetRunes;
    }

    public void ForceUpdate() => SetRunes();

    private void SetRunes()
    {
        for (int i = 0; i < m_MiniRuneIcons.Length; i++)
        {
            if (i < RunesContainer.CurrentRuneEffects.Count && 
                RunesContainer.CurrentRuneEffects[i] is BaseRuneEffect runeEffect)
            {
                m_MiniRuneIcons[i].enabled = true;
                m_MiniRuneIcons[i].sprite = runeEffect.Icon;
            }
            else
            {
                m_MiniRuneIcons[i].enabled = false;
            }
        }
        for (int i = 0; i < m_RunePanels.Length; i++)
        {
            if (i < RunesContainer.CurrentRuneEffects.Count && 
                RunesContainer.CurrentRuneEffects[i] is BaseRuneEffect runeEffect)
            {
                if(i > 0)
                    m_RunePanels[i].SetRune(runeEffect, canBeDiscarded: true, canBeSendOut: OpenWithStorage);
                else
                    m_RunePanels[i].SetRune(runeEffect);
            }
            else
            {
                m_RunePanels[i].SetPanelEmpty();
            }
        }
    }

    public void ResetAllRunePanelsExcept(RunePanel panel)
    {
        if (m_StartDescription != null)
        {
            Destroy(m_StartDescription.gameObject);
        }
        foreach (var runePanel in m_RunePanels)
        {
            if (panel == runePanel) continue;
            runePanel.ManualSetState(false);
        }
    }
}
