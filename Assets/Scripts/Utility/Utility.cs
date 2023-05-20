using DG.Tweening;
using GabrielBigardi.SpriteAnimator.Runtime;
using UnityEngine;

public static class Utility
{
    public static void DashMove(this Transform transform, Vector2 direction, float distance = 1, float duration = 0.5f)
    {
        if (direction == Vector2.zero) direction = Vector2.right;
        var destination = transform.position + (Vector3)direction * distance;
        transform.DOKill();
        transform.DOMove(destination, duration).SetEase(Ease.Linear);
    }

    public static float GetAnimationTime(this SpriteAnimation spriteAnimation)
    {
        return (float)spriteAnimation.Frames.Count / spriteAnimation.FPS;
    }
}
