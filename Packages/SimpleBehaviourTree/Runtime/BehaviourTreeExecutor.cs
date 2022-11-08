using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static PlasticPipe.PlasticProtocol.Messages.Serialization.ItemHandlerMessagesSerialization;

namespace SimpleBehaviourTree
{
    public class BehaviourTreeExecutor : MonoBehaviour
    {
        [SerializeField] private BehaviourTree m_BehaviourTree;

        public BehaviourTree BehaviourTree => m_BehaviourTree;

        private void Start()
        {
            if (m_BehaviourTree != null)
            {
                try
                {
                    m_BehaviourTree = m_BehaviourTree.Clone(gameObject);
                    m_BehaviourTree.blackboard.ConvertSavedListToDictionary();
                }
                catch (System.Exception)
                {
                    m_BehaviourTree = null;
                }
            }
        }

        private void Update()
        {
            if (m_BehaviourTree != null)
                m_BehaviourTree.Update();
        }
    }
}