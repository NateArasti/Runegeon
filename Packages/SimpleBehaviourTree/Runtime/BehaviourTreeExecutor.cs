using UnityEngine;

namespace SimpleBehaviourTree
{
    public class BehaviourTreeExecutor : MonoBehaviour
    {
        [SerializeField] private BehaviourTree _behaviourTree;

        public BehaviourTree BehaviourTree => _behaviourTree;

        private void Start()
        {
            _behaviourTree = _behaviourTree.Clone();
        }

        private void Update()
        {
            _behaviourTree.Update();
        }
    }
}