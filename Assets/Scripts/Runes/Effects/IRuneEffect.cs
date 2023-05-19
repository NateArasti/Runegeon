public interface IRuneEffect
{
    void OnMove();

    void OnRoll();

    void OnAttack(IAttackProvider attackProvider, IAttackReciever attackReciever);

    void OnHit();
}
