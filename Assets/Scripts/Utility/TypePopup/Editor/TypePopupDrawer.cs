using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TypePopupAttribute))]
public class TypePopupDrawer : PropertyDrawer
{
    private const string NullVariant = "Null";

    private bool m_Start = true;
    private string[] m_Types;

    private TypePopupAttribute TypePopup => (TypePopupAttribute)attribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(m_Start)
        {
            m_Start = false;

            var typeCollection = TypeCache.GetTypesDerivedFrom(TypePopup.Type);
            m_Types = new string[typeCollection.Count + 1];
            m_Types[0] = NullVariant;
            for (int i = 1; i < m_Types.Length; i++)
            {
                m_Types[i] = typeCollection[i - 1].Name;
            }

            for (int i = 0; i < m_Types.Length; i++)
            {
                if (m_Types[i].Equals(property.stringValue))
                {
                    TypePopup.Selected = i;
                    break;
                }
            }
        }

        TypePopup.Selected = EditorGUI.Popup(
            EditorGUI.PrefixLabel(position, label), 
            TypePopup.Selected,
            m_Types);

        property.stringValue = 
            m_Types[TypePopup.Selected].Equals(NullVariant) ? 
            string.Empty : 
            m_Types[TypePopup.Selected];
    }
}
