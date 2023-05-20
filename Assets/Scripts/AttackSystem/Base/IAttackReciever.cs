public interface IAttackReciever
{
    public bool Active { get; }

    void RecieveAttack(IAttackProvider attackProvider);
}
