public interface IAttacker
{
    public float AttackDuration { get; }
    public float AttackCooldown { get; }

    public void GoToTarget();

    public bool IsTargetInRange();

    public void StepBack(float stepBackTime);

    public void Attack();
}