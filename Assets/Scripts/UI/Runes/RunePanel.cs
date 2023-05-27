using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunePanel : DoubleStateObject
{
    [SerializeField] private Sprite m_DefaultRuneIcon;
    [SerializeField] private Button m_RuneButton;
    [SerializeField] private GameObject m_DiscardButton;
    [SerializeField] private GameObject m_SendOutButton;
    [SerializeField] private GameObject m_GrabButton;
    [SerializeField] private Image m_Image;
    [SerializeField] private TMP_Text m_Desctiption;

    private BaseRuneEffect m_Rune;

    private void Awake()
    {
        SetPanelEmpty();

        OnStateSet += OnPanelStateSet;
    }

    public void SetRune(BaseRuneEffect runeEffect, bool canBeSendOut = false, bool canBeGrabbed = false, bool canBeDiscarded = false)
    {
        if (runeEffect == null) return;
        m_Rune = runeEffect;
        m_Image.color = Color.white;
        m_Image.sprite = m_Rune.Icon;
        m_RuneButton.interactable = true;
        m_DiscardButton.SetActive(canBeDiscarded);
        m_SendOutButton.SetActive(canBeSendOut);
        m_GrabButton.SetActive(canBeGrabbed);
    }

    public void SendOutRune()
    {
        if (m_Rune == null) return;
        ManualSetState(false);
        RunesContainer.DiscardRuneFromTarget(m_Rune, GlobalPlayerData.PlayerTransform.gameObject);
        RunesContainer.DiscardRuneToStorage(m_Rune);
    }

    public void ClaimRuneFromStorage()
    {
        if (m_Rune == null) return;
        ManualSetState(false);
        RunesContainer.ClaimRuneFromStorage(m_Rune);
        RunesContainer.ApplyRuneOnTarget(m_Rune, GlobalPlayerData.PlayerTransform.gameObject);
    }

    public void DiscardRune()
    {
        if (m_Rune == null) return;
        ManualSetState(false);
        RunesContainer.DiscardRuneFromTarget(m_Rune, GlobalPlayerData.PlayerTransform.gameObject);
        RunesContainer.RemoveRuneEffect(m_Rune);
    }

    public void SetPanelEmpty()
    {
        m_Image.color = Color.black;
        m_Image.sprite = m_DefaultRuneIcon;
        m_RuneButton.interactable = false;
        m_DiscardButton.SetActive(false);
        m_SendOutButton.SetActive(false);
        m_GrabButton.SetActive(false);
        ManualSetState(false);
    }

    private void OnPanelStateSet(bool state)
    {
        if (state)
        {
            m_Desctiption.text = m_Rune.Description;
        }
    }
}
