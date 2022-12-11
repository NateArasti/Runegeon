namespace SimpleBehaviourTree
{
    public class SelectorNode : CompositionNode
    {
        private int m_CurrentChildIndex;

        public override string DisplayName => "Selector";

        protected override State OnUpdate()
        {
            m_CurrentChildIndex = 0;
            if (m_CurrentChildIndex >= children.Count) return State.Failure;
            State currentChildState;
            do
            {
                currentChildState = children[m_CurrentChildIndex].Update();
                if(currentChildState == State.Failure)
                    children[m_CurrentChildIndex].DiscardState();
                m_CurrentChildIndex++;
            } while (currentChildState == State.Failure);

            return currentChildState;
        }
    }
}