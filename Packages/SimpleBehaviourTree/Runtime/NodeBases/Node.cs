using UnityEditor;
using UnityEngine;

namespace SimpleBehaviourTree
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running, 
            Success,
            Failure
        }

        [HideInInspector] public string nodeGUID;
        [HideInInspector] public Vector2 GraphPosition;

        private bool _started;

        public State NodeState { get; private set; }
        public bool Started => _started;

        public virtual string DisplayName => "Node";

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        internal State Update()
        {
            if (!_started)
            {
                _started = true;
                OnStart();
            }

            NodeState = OnUpdate();

            if(NodeState != State.Running)
            {
                _started = false;
                OnStop();
            }

            return NodeState;
        }

        protected abstract State OnUpdate();

        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
    }
}