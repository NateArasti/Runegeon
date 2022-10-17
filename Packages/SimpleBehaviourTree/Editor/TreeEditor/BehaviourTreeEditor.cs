using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleBehaviourTree
{
    internal class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeView _treeView;
        private BehaviourTreeInspector _treeInspector;
        private BlackboardView _blackboardView;

        [MenuItem("SimpleBehaviourTree/Open BehaviourTreeEditor")]
        public static void OpenEditor()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
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

            _treeInspector = root.Q<BehaviourTreeInspector>();
            _treeView = root.Q<BehaviourTreeView>();
            _blackboardView = root.Q<BlackboardView>();

            _blackboardView.onGUIHandler += _blackboardView.HandleGUI;

            _treeView.OnNodeSelected += OnNodeSelectionChange;
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
            var tree = Selection.activeObject as BehaviourTree;

            if(tree == null && 
                Selection.activeGameObject != null &&
                Selection.activeGameObject.TryGetComponent<BehaviourTreeExecutor>(out var executor))
            {
                tree = executor.BehaviourTree;
            }

            if(_treeView != null && tree != null && 
                (Application.isPlaying || AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())))
            {
                _treeView.PopulateView(tree);
            }
        }

        private void OnInspectorUpdate()
        {
            _treeView.UpdateNodeStates();
        }

        private void OnNodeSelectionChange(NodeView nodeView)
        {
            _treeInspector.UpdateNodeSelection(nodeView);
        }
    }
}