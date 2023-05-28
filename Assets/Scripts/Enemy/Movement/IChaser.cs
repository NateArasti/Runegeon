public interface IChaser
{
    public enum ChaseRange
    {
        Close,
        Distant
    }

    public bool Chasing { get; set; }

    public bool AtTargetRange { get; }

    public void StayInRange(ChaseRange chaseRange);
    public float GetStayInRangeTime(ChaseRange chaseRange);
}