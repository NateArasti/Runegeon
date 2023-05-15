using UnityEngine;

namespace SimpleBehaviourTree
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            None,
            Running, 
            Success,
            Failure
        }

        [HideInInspector] public string nodeGUID;
        [HideInInspector] public Vector2 GraphPosition;

        protected GameObject m_ExecutorObject;
        protected Blackboard m_Blackboard;
        private bool m_Started;

        public State NodeState { get; private set; }
        public bool Started => m_Started;

        public virtual string DisplayName => "Node";

        internal virtual Node Clone(GameObject executorObject, Blackboard blackboard)
        {
            var node = Instantiate(this);
            node.m_ExecutorObject = executorObject;
            node.m_Blackboard = blackboard;
            return node;
        }

        internal virtual void DiscardState()
        {
            NodeState = State.None;
            m_Started = false;
            OnStop();
        }

        internal State Update()
        {
            if (!m_Started)
            {
                m_Started = true;
                OnStart();
            }

            NodeState = OnUpdate();

            if(NodeState != State.Running)
            {
                m_Started = false;
                OnStop();
            }

            return NodeState;
        }

        protected abstract State OnUpdate();

        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
    }
}