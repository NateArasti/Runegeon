public interface IAttacker
{
    public float AttackDuration { get; }
    public float AttackCooldown { get; }

    public void GoToTarget();

    public bool IsTargetInRange();

    public void StepBack();

    public void Attack();
}