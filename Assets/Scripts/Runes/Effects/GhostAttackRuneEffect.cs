using GabrielBigardi.SpriteAnimator.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

[CreateAssetMenu(fileName = "GhostAttackEffect", menuName = "RuneEffects/GhostAttackEffect")]
public class GhostAttackRuneEffect : BaseRuneEffect
{
    [SerializeField, Range(0, 1)] private float m_EffectChance = 0.3f;
    [SerializeField] private SpriteAnimator m_AttackGhost;
    [SerializeField] private float m_SpawnDelay = 0.5f;
    [SerializeField] private float m_SpawnOffsetDistance = 1;
    [SerializeField] private SpriteAnimation[] m_PossibleAttackAnimations;

    private StatsContainer m_PlayerStatsContainer;

    public override void OnApply(GameObject target)
    {
        m_PlayerStatsContainer = target.GetComponent<StatsContainer>();
    }

    public override void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever)
    {
        if(Random.value > m_EffectChance) return;

        if(attackProvider is not MonoBehaviour provider ||
            attackReciever is not MonoBehaviour reciever) return;

        var spawnOffsetDirection = GetSpawnOffsetDirection(provider.transform.root, reciever.transform.root);

        CoroutineExtensions.InvokeSecondsDelayed(() =>
        {

            var ghost = Instantiate(
                m_AttackGhost,
                reciever.transform.root.position + m_SpawnOffsetDistance * spawnOffsetDirection,
                Quaternion.identity);

            var scale = ghost.transform.localScale;
            scale.x *= -spawnOffsetDirection.x;
            ghost.transform.localScale = scale;

            var ghostAttackProvider = ghost.GetComponentInChildren<AttackProvider>(true);
            ghostAttackProvider.OnSuccessAttack.AddListener(RunesContainer.ApplyAttackEffects);
            ghostAttackProvider.Damage = m_PlayerStatsContainer.CurrentStats.AttackDamage;
            var attackAnimation = m_PossibleAttackAnimations.GetRandomObject();

            ghost.Play(attackAnimation);

            CoroutineExtensions.InvokeSecondsDelayed(
                () => Destroy(ghost.gameObject),
                attackAnimation.GetAnimationTime()
                );
        }, m_SpawnDelay);
    }

    private Vector3 GetSpawnOffsetDirection(Transform attackProvider, Transform attackReciever)
    {
        var possibleDirections = new List<Vector3>();
        var delta = attackProvider.position - attackReciever.position;
        foreach (var direction in new[] { Vector3.right, Vector3.left })
        {
            if(Vector3.Dot(delta, direction) < 0.5f)
            {
                possibleDirections.Add(direction);
            }
        }
        return possibleDirections.GetRandomObject();
    }
}
