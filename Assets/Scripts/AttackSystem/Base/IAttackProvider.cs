public interface IAttackProvider
{
    public bool Active { get; }

    public int Damage { get; }

    void OnSuccessHit(IAttackReciever reciever);
}
