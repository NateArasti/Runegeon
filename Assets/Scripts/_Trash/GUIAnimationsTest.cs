using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

[RequireComponent(typeof(Animator))]
public class GUIAnimationsTest : MonoBehaviour
{
    [SerializeField] private List<string> _animationStates = new();
    private Animator _animator;

    private void Awake()
    {
        if(!TryGetComponent(out _animator)) Debug.LogError($"No animator on {name}");
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        _animationStates.ForEachAction(state =>
        {
            if (GUILayout.Button($"Play {state}"))
            {
                PlayAnimation(state);
            }
        });
        GUILayout.EndVertical();
    }

    private void PlayAnimation(string state)
    {
        _animator.Play(state);
    }
}
