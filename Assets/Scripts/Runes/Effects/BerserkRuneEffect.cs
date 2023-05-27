using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityExtensions;
using DG.Tweening;

[CreateAssetMenu(fileName = "BerserkEffect", menuName = "RuneEffects/BerserkEffect")]
public class BerserkRuneEffect : BaseRuneEffect
{
    [Space]
    [SerializeField, Range(0, 1)] private float m_ProgressIncreasePerHit = 0.25f;
    [SerializeField] private float m_BerserkModeTime = 5;
    [SerializeField] private float m_BerserkLoseSpeed = 0.1f;
    [Header("Effect")]
    [SerializeField] private Volume m_BerserkVolumePrefab;
    [SerializeField] private float m_VolumeLoseThreshold = 0.5f;
    [SerializeField] private float m_VolumeGainThreshold = 0.3f;
    [SerializeField] private float m_SkipVolumeDuration = 0.5f;
    [Header("Modificators")]
    [SerializeField] private float m_AttackSpeedIncrease = 0.1f;
    [SerializeField] private int m_AttackDamageIncrese = 2;
    [SerializeField] private float m_MoveSpeedIncrease = 2f;

    private bool m_Applied;
    private StatsContainer m_StatsContainer;

    private float m_CurrentVolumeSetDelay;
    private Volume m_VolumeInstance;

    private bool m_InBerserkMode;
    private float m_CurrentBerserkProgress;
    private Coroutine m_UpdateCoroutine;

    public override void OnApply(GameObject target)
    {
        m_Applied = true;

        m_StatsContainer = target.GetComponent<StatsContainer>();
        m_VolumeInstance = Instantiate(m_BerserkVolumePrefab, target.transform);
        m_VolumeInstance.weight = 0;

        m_CurrentBerserkProgress = 0;
        m_UpdateCoroutine = CoroutineExtensions.SeparateCoroutineBehaviour.StartCoroutine(UpdateRoutine());
    }

    public override void OnDiscard(GameObject target)
    {
        m_Applied = false;

        Destroy(m_VolumeInstance.gameObject);

        m_CurrentBerserkProgress = 0;
        ExitBerserkMode();
        CoroutineExtensions.SeparateCoroutineBehaviour.StopCoroutine(m_UpdateCoroutine);
    }

    public override void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever)
    {
        if (!m_Applied) return;

        if (m_InBerserkMode)
        {
            CameraShakeEffect.Instance.Play();
            return;
        }

        m_CurrentBerserkProgress += m_ProgressIncreasePerHit;

        if(m_CurrentBerserkProgress > 1)
        {
            m_CurrentBerserkProgress = 1;
            EnterBerserkMode();
        }
    }

    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            if(m_CurrentVolumeSetDelay > 0) m_CurrentVolumeSetDelay -= Time.deltaTime;

            if (m_InBerserkMode)
            {
                m_CurrentBerserkProgress -= Time.deltaTime / m_BerserkModeTime;

                if(m_CurrentVolumeSetDelay <= 0)
                {
                    m_VolumeInstance.weight = 
                        m_VolumeLoseThreshold + m_CurrentBerserkProgress * (1 - m_VolumeLoseThreshold);
                }
            }
            else
            {
                m_CurrentBerserkProgress -= m_BerserkLoseSpeed * Time.deltaTime;
                if (m_CurrentVolumeSetDelay <= 0)
                {
                    m_VolumeInstance.weight = m_CurrentBerserkProgress * m_VolumeGainThreshold;
                }
            }
            m_CurrentBerserkProgress = Mathf.Clamp01(m_CurrentBerserkProgress);
            if(m_CurrentBerserkProgress == 0 && m_InBerserkMode)
            {
                ExitBerserkMode();
            }
            yield return null;
        }
    }

    private void EnterBerserkMode()
    {
        m_InBerserkMode = true;

        PlayerHUD.Instance.gameObject.SetActive(false);

        m_CurrentVolumeSetDelay = m_SkipVolumeDuration;
        DOTween.To(
            () => m_VolumeInstance.weight, 
            (v) => m_VolumeInstance.weight = v, 
            1, m_SkipVolumeDuration)
            .SetEase(Ease.Linear);

        var modificatedStats = new ModificatedStats(
            m_StatsContainer.CurrentStats, 
            m_MoveSpeedIncrease, 
            m_AttackSpeedIncrease, 
            m_AttackDamageIncrese
            );
        m_StatsContainer.SetModification(modificatedStats);
    }

    private void ExitBerserkMode()
    {
        m_InBerserkMode = false;

        PlayerHUD.Instance.gameObject.SetActive(true);

        m_CurrentVolumeSetDelay = m_SkipVolumeDuration;
        DOTween.To(
            () => m_VolumeInstance.weight, 
            (v) => m_VolumeInstance.weight = v, 
            0, m_SkipVolumeDuration)
            .SetEase(Ease.Linear);

        m_StatsContainer.DiscardModifications();
    }
}
