using BehavioursRectangularGraph;
using GabrielBigardi.SpriteAnimator.Runtime;
using QuickEye.Utility;
using UnityEngine;
using UnityExtensions;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private SpriteAnimator m_SpriteAnimator;
    [SerializeField] private UnityDictionary<RectangularDirection, SpriteAnimation> m_IDLEAnimations;
    [SerializeField] private UnityDictionary<RectangularDirection, SpriteAnimation> m_WalkAnimations;
    [SerializeField] private UnityDictionary<RectangularDirection, SpriteAnimation> m_DodgeAnimations;
    [SerializeField] private UnityDictionary<RectangularDirection, SpriteAnimation>[] m_AttackAnimations;
    [Space]
    [SerializeField] private PlayerVisuals[] m_ConnectedVisuals;

    public int AttackComboCount => m_AttackAnimations.Length;

    public SpriteAnimation PlayDodgeAnimation(RectangularDirection direction, float targetTime = -1)
    {
        if (!m_DodgeAnimations.ContainsKey(direction)) return null;

        var animation = m_DodgeAnimations[direction];
        m_SpriteAnimator.PlayIfNotPlaying(animation);

        if (targetTime > 0)
        {
            animation.FPS = (int)(animation.Frames.Count / targetTime);
        }

        m_ConnectedVisuals.ForEachAction(visuals =>
        {
            if (visuals == this) return;
            visuals.PlayDodgeAnimation(direction, targetTime);
        });

        return animation;
    }

    public SpriteAnimation PlayIDLEAnimation(RectangularDirection direction)
    {
        if (!m_IDLEAnimations.ContainsKey(direction)) return null;

        var animation = m_IDLEAnimations[direction];
        m_SpriteAnimator.PlayIfNotPlaying(animation);

        m_ConnectedVisuals.ForEachAction(visuals =>
        {
            if (visuals == this) return;
            visuals.PlayIDLEAnimation(direction);
        });

        return animation;
    }

    public SpriteAnimation PlayWalkAnimation(RectangularDirection direction)
    {
        if (!m_WalkAnimations.ContainsKey(direction)) return null;

        var animation = m_WalkAnimations[direction];
        m_SpriteAnimator.PlayIfNotPlaying(animation);

        m_ConnectedVisuals.ForEachAction(visuals =>
        {
            if (visuals == this) return;
            visuals.PlayWalkAnimation(direction);
        });

        return animation;
    }

    public SpriteAnimation PlayAttackAnimation(int attackIndex, RectangularDirection direction, float targetTime = -1)
    {
        if (!m_AttackAnimations[attackIndex].ContainsKey(direction)) return null;

        var animation = m_AttackAnimations[attackIndex][direction];
        m_SpriteAnimator.Play(animation);

        if(targetTime > 0)
        {
            animation.FPS = (int)(animation.Frames.Count / targetTime);
        }

        m_ConnectedVisuals.ForEachAction(visuals =>
        {
            if (visuals == this) return;
            visuals.PlayAttackAnimation(attackIndex, direction, targetTime);
        });

        return animation;
    }
}
