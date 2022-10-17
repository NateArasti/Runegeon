using UnityEngine.UIElements;

public class SplitView : TwoPaneSplitView
{
    public class BehaviorTreeUxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }

    public SplitView() { }
}
