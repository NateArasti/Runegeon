namespace SimpleBehaviourTree
{
    public class MoveFromNode : ActionNode
    {
        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}