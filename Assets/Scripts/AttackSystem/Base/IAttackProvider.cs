public interface IAttackProvider
{
    public bool Active { get; }

    public float Damage { get; }

    void OnSuccessHit(IAttackReciever reciever);
}
