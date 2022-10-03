using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class RoomsCompatableCheck : MonoBehaviour
{
    [SerializeField] private Room _firstRoom;
    [SerializeField] private Room _secondRoom;

    [SerializeField, ReadOnly] private bool _isCompatable;

    private void Update()
    {
        if (_firstRoom != null && _secondRoom != null)
        {
            _isCompatable = _firstRoom.IsCompatable(
                _firstRoom.transform.position,
                _secondRoom,
                _secondRoom.transform.position);
        }
    }
}
