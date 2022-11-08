using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleBehaviourTree
{
    internal class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTree m_Tree;
        private BehaviourTreeView m_TreeView;
        private BehaviourTreeInspector m_TreeInspector;
        private BlackboardView m_BlackboardView;

        [MenuItem("SimpleBehaviourTree/Open BehaviourTreeEditor")]
        public static void OpenEditor()
        {
            var window = GetWindow<BehaviourTreeEditor>();
            window.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if(Selection.activeObject is BehaviourTree)
            {
                OpenEditor();
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = BehaviourTreeSettings.BehaviourTreeUXML;
            visualTree.CloneTree(root);

            var styleSheet = BehaviourTreeSettings.BehaviourTreeUSS;
            root.styleSheets.Add(styleSheet);

            m_TreeInspector = root.Q<BehaviourTreeInspector>();
            m_TreeView = root.Q<BehaviourTreeView>();
            m_BlackboardView = root.Q<BlackboardView>();
            m_BlackboardView.onGUIHandler += () =>
            {
                EditorGUI.BeginChangeCheck();
                m_BlackboardView.HandleGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(m_Tree);
                    AssetDatabase.SaveAssetIfDirty(m_Tree);
                }
            };

            m_TreeView.OnNodeSelected += OnNodeSelectionChange;
            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
        }

        private void HandlePlayModeStateChange(PlayModeStateChange playModeState)
        {
            switch (playModeState)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange()
        {
            m_Tree = Selection.activeObject as BehaviourTree;

            if (m_Tree == null && 
                Selection.activeGameObject != null &&
                Selection.activeGameObject.TryGetComponent<BehaviourTreeExecutor>(out var executor))
            {
                m_Tree = executor.BehaviourTree;
            }

            if(m_TreeView != null && m_Tree != null && 
                (Application.isPlaying || AssetDatabase.CanOpenAssetInEditor(m_Tree.GetInstanceID())))
            {
                m_TreeView.PopulateView(m_Tree);
                m_Tree.blackboard.ConvertSavedListToDictionary();
                m_BlackboardView.Blackboard = m_Tree.blackboard;
            }
        }

        private void OnInspectorUpdate()
        {
            m_TreeView.UpdateNodeStates();
        }

        private void OnNodeSelectionChange(NodeView nodeView)
        {
            m_TreeInspector.UpdateNodeSelection(nodeView);
        }
    }
}