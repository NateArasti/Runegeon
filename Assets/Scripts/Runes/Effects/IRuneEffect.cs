public interface IRuneEffect
{
    void OnMove();

    void OnRoll();

    void OnAttack(IAttackReciever attackReciever);

    void OnHit();
}
