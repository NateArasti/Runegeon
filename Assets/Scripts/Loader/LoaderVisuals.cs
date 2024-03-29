using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class LoaderVisuals : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectMask2D m_Mask;
    [SerializeField] private float m_LoadingSpeed = 1;
    private float m_MaxMaskValue;
    private bool m_Loading;

    public bool Loading
    {
        get => m_Loading; 
        set
        {
            m_Loading = value;
            m_CanvasGroup.blocksRaycasts = value;
        }
    }

    private void Awake()
    {
        m_CanvasGroup.alpha = 0;
        Loading = false;
        m_MaxMaskValue = m_Mask.rectTransform.rect.width;

        Loader.OnLoadingStart += StartLoading;
        Loader.OnLoadingEnd += EndLoading;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            if (Loading)
            {
                var targetMaskValue = m_MaxMaskValue - Loader.CurrentLoadProgress * m_MaxMaskValue;
                var smoothMaskValue = Mathf.MoveTowards(
                    m_Mask.padding.z,
                    targetMaskValue,
                    Time.unscaledDeltaTime * m_LoadingSpeed);
                SetMaskValue(smoothMaskValue);
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        Loader.OnLoadingStart -= StartLoading;
        Loader.OnLoadingEnd -= EndLoading;
    }

    private void StartLoading()
    {
        m_CanvasGroup.DOFade(1, 1).SetUpdate(true);
        Loading = true;
        SetMaskValue(m_MaxMaskValue);
    }

    private void EndLoading()
    {
        CoroutineExtensions.InvokeAfter(
            () => m_Mask.padding.z <= (1 - Loader.k_ActivateThreshold) * m_MaxMaskValue,
            () =>
            {
                Loader.ActivateLoadedScene();
                CoroutineExtensions.InvokeRealtimeSecondsDelayed(
                    () =>
                    {
                        Loading = false;
                        m_CanvasGroup.DOFade(0, 1).SetUpdate(true);
                    }, 5);
            });
    }

    private void SetMaskValue(float value) =>
        m_Mask.padding = new Vector4(0, 0, value, 0);
}
