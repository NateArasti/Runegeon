using System;
using System.Linq;
using UnityEngine;

public class RunesStorage : MonoBehaviour
{
    [SerializeField] private BaseRuneEffect[] m_RequiredRuneEffects;
    [SerializeField] private RunePanel[] m_RunePanels;
    [SerializeField] private GameObject m_StartDescription;

    private void Start()
    {
        RunesContainer.OnChange += SetRunes;
        SetRunes();
    }

    private void SetRunes()
    {
        var i = 0;
        for (; i < m_RunePanels.Length && i < m_RequiredRuneEffects.Length; i++)
        {
            var rune = m_RequiredRuneEffects[i];
            if (RunesContainer.CurrentRuneEffects.Contains(rune))
            {
                m_RunePanels[i].SetRune(rune, canBeSendOut: true);
            }
            else if (RunesContainer.InStorage(rune))
            {
                m_RunePanels[i].SetRune(rune, canBeGrabbed: true);
            }
            else
            {
                m_RunePanels[i].SetPanelEmpty();
            }
        }
    }

    public void ResetAllRunePanelsExcept(RunePanel panel)
    {
        if(m_StartDescription != null)
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
