namespace SimpleBehaviourTree
{
    public class MoveToNode : ActionNode
    {
        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}