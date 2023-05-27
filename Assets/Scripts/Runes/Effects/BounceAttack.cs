using System;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

public class BounceAttack : IAttackProvider
{
    public event Action<Vector3, Vector3> OnAttack;

    private readonly BounceAttackData m_AttackData;

    private int m_CurrentBounceCount;

    public bool Active => true;

    public int Damage => m_AttackData.BounceDamage;

    public BounceAttack(BounceAttackData attackData)
    {
        m_CurrentBounceCount = 0;
        m_AttackData = attackData;
    }

    public void OnSuccessHit(IAttackReciever reciever)
    {
        if (m_CurrentBounceCount >= m_AttackData.MaxBounceCount) return;
        CoroutineExtensions.InvokeSecondsDelayed(
            () => ProvideAttack(reciever),
            m_AttackData.BounceDelay
            );
    }

    public void ProvideAttack(IAttackReciever previousReciever)
    {
        if (previousReciever == null) return;

        var center = previousReciever is MonoBehaviour behaviour ?
            behaviour.transform.position :
            Vector3.zero;
        var closeTargets =
            Physics2D.OverlapCircleAll(center, m_AttackData.BounceRange, m_AttackData.BounceMask);
        var possibleRecievers = new HashSet<IAttackReciever>();
        foreach (var possibleTarget in closeTargets)
        {
            var possibleReciever = possibleTarget.GetComponent<IAttackReciever>();
            if (possibleReciever != null && previousReciever != possibleReciever)
            {
                possibleRecievers.Add(possibleReciever);
            }
        }

        var nextReciever = possibleRecievers.Count > 0 ? possibleRecievers.GetRandomObject() : null;

        if (nextReciever == null) return;

        m_CurrentBounceCount++;

        var previousRecieverPosition = (previousReciever as MonoBehaviour).transform.position;
        var nextRecieverPosition = (nextReciever as MonoBehaviour).transform.position;

        OnAttack?.Invoke(previousRecieverPosition, nextRecieverPosition);

        nextReciever.RecieveAttack(this);
    }

    [System.Serializable]
    public class BounceAttackData
    {
        [SerializeField] private float m_BounceDelay = 0.5f;
        [SerializeField] private float m_CollapseDelay = 0.3f;
        [SerializeField] private int m_MaxBounceCount = 3;
        [SerializeField] private float m_BounceRange = 1f;
        [SerializeField] private int m_BounceDamage = 10;
        [SerializeField] private LayerMask m_BounceMask;

        public float BounceDelay => m_BounceDelay;
        public float CollapseDelay => m_CollapseDelay;
        public int MaxBounceCount => m_MaxBounceCount;
        public float BounceRange => m_BounceRange;
        public int BounceDamage => m_BounceDamage;
        public LayerMask BounceMask => m_BounceMask;
    }
}
