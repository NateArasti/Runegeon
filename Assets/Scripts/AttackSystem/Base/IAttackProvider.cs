public interface IAttackProvider
{
    public float Damage { get; }

    void OnSuccessHit(IAttackReciever reciever);
}
