namespace SimpleBehaviourTree
{
    public class SelectorNode : CompositionNode
    {
        public override string DisplayName => "Selector";

        protected override State OnUpdate()
        {
            var currentChildState = State.Failure;
            for (var i = 0; i < children.Count; i++)
            {
                if(currentChildState != State.Failure)
                {
                    children[i].DiscardState();
                }
                else
                {
                    currentChildState = children[i].Update();
                    if (currentChildState == State.Failure)
                        children[i].DiscardState();
                }
            }
            return currentChildState;
        }
    }
}