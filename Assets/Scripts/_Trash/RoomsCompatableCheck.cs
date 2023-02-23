using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class RoomsCompatableCheck : MonoBehaviour
{
    [SerializeField] private TilemapCollider2D _firstRoom;
    [SerializeField] private TilemapCollider2D _secondRoom;

    [SerializeField, ReadOnly] private bool _isCompatable;

    private void Update()
    {
        if (_firstRoom != null && _secondRoom != null)
        {
            _isCompatable = _firstRoom.IsTouching(_secondRoom);
        }
    }
}
