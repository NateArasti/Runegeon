using UnityEngine;

namespace SimpleBehaviourTree
{
    public class SequenceNode : CompositionNode
    {
        private int _currentNode;

        public override string DisplayName => "Sequence";

        protected override void OnStart()
        {
            _currentNode = 0;
        }

        protected override State OnUpdate()
        {
            if (_currentNode >= children.Count) 
            {
                Debug.LogWarning($"{name} got out of the children nodes bounds");
                return State.Failure;
            }
            var state = children[_currentNode].Update();
            if(state == State.Success)
            {
                _currentNode++;
                return _currentNode == children.Count ? State.Success : State.Running;
            }
            return state;
        }
    }
}