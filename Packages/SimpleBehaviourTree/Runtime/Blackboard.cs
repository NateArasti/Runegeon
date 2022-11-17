using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleBehaviourTree
{
    [Serializable]
    public class Blackboard
    {
        public static IReadOnlyList<Type> AvaiableTypes = new List<Type>
    {
        typeof(int),
        typeof(float),
        typeof(string),
        typeof(bool),
    };

        public static string[] TypeNames => AvaiableTypes.Select(t => t.Name).ToArray();

        [Serializable]
        public struct VariableData
        {
            public string Name;
            public byte[] Value;
        }

        [SerializeField] private List<VariableData> m_VariableDatas = new();

        public Dictionary<string, object> Data { get; } = new();

        public void ConvertSavedListToDictionary()
        {
            Data.Clear();
            foreach (var variableData in m_VariableDatas)
            {
                var value = Utility.ByteArrayToObject(variableData.Value);
                Data.Add(variableData.Name, value);
            }
        }

        public void AddObjectToData(string fieldName, Type type)
        {
            if (!AvaiableTypes.Contains(type)) return;
            if (Data.ContainsKey(fieldName))
            {
                Debug.LogWarning("Tree already have variable with this name");
                while (Data.ContainsKey(fieldName))
                {
                    fieldName += "(1)";
                }
            }
            if (type.IsValueType)
            {
                Data.Add(fieldName, Activator.CreateInstance(type));
            }
            else if (type == typeof(string))
            {
                Data.Add(fieldName, string.Empty);
            }

            m_VariableDatas.Add(new VariableData
            {
                Name = fieldName,
                Value = Utility.ObjectToByteArray(Data[fieldName])
            });
        }

        #region VariablesControl

        #region Base

        private bool TryGet(string name, out object result, Type type)
        {
            if (Data.TryGetValue(name, out var value) && IsObjectOfType(value, type))
            {
                result = value;
                return true;
            }
            result = default;
            return false;
        }

        private bool TrySet(string name, object value, Type type)
        {
            if (Data.ContainsKey(name) && IsObjectOfType(value, type))
            {
                Data[name] = value;
                return true;
            }
            return false;
        }

        #endregion

        #region Int

        public bool TryGetInt(string name, out int result)
        {
            if(TryGet(name, out var obj, typeof(int)))
            {
                result = (int)obj;
                return true;
            }
            result = default;
            return false;
        }

        public bool TrySetInt(string name, int value)
        {
            return TrySet(name, value, typeof(int));
        }
        #endregion

        #region Float

        public bool TryGetFloat(string name, out float result)
        {
            if(TryGet(name, out var obj, typeof(float)))
            {
                result = (float)obj;
                return true;
            }
            result = default;
            return false;
        }

        public bool TrySetFloat(string name, float value)
        {
            return TrySet(name, value, typeof(float));
        }

        #endregion

        #region Bool

        public bool TryGetBool(string name, out bool result)
        {
            if(TryGet(name, out var obj, typeof(bool)))
            {
                result = (bool)obj;
                return true;
            }
            result = default;
            return false;
        }

        public bool TrySetBool(string name, bool value)
        {
            return TrySet(name, value, typeof(bool));
        }

        #endregion

        #region String

        public bool TryGetString(string name, out string result)
        {
            if(TryGet(name, out var obj, typeof(string)))
            {
                result = (string)obj;
                return true;
            }
            result = default;
            return false;
        }

        public bool TrySetString(string name, string value)
        {
            return TrySet(name, value, typeof(string));
        }

        #endregion

        private static bool IsObjectOfType(object obj, Type type)
        {
            if (!AvaiableTypes.Contains(type)) return false;
            return type.IsInstanceOfType(obj);
        }

        #endregion

#if UNITY_EDITOR

        public void DrawGUI()
        {
            for (int i = 0; i < m_VariableDatas.Count; i++)
            {
                var variableData = m_VariableDatas[i];
                var key = variableData.Name;
                if (!Data.ContainsKey(key))
                {
                    Debug.LogError($"Converted data doesn't contains value {key}");
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    m_VariableDatas.Remove(m_VariableDatas[i]);
                    Data.Remove(key);
                    continue;
                }
                EditorGUILayout.LabelField($"{key} ({Data[key].GetType().Name})");
                if (Data[key] is int @int)
                {
                    Data[key] = EditorGUILayout.IntField(@int);
                }
                else if (Data[key] is float @float)
                {
                    Data[key] = EditorGUILayout.FloatField(@float);
                }
                else if (Data[key] is string @string)
                {
                    Data[key] = EditorGUILayout.TextField(@string);
                }
                else if (Data[key] is bool @bool)
                {
                    Data[key] = EditorGUILayout.Toggle(@bool);
                }
                variableData.Value = Utility.ObjectToByteArray(Data[key]);
                m_VariableDatas[i] = variableData;
                EditorGUILayout.EndHorizontal();
            }
        }

#endif
    }
}