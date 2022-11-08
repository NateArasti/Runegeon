using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace SimpleBehaviourTree
{
    public static class Utility
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteArrayToObject(byte[] bytes)
        {
            if (bytes == null)
                return null;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(bytes))
            {
                return bf.Deserialize(ms);
            }
        }

#if UNITY_EDITOR

        private static readonly Dictionary<Type, string> _cachedScriptPaths = new();

        public static string GetScriptPath(Type type, bool forceSearch = false)
        {
            if (!forceSearch && _cachedScriptPaths.ContainsKey(type))
            {
                return _cachedScriptPaths[type];
            }

            var guids = AssetDatabase.FindAssets(string.Format("{0} t:script", type.Name));

            if (guids.Length == 1)
            {
                return AssetDatabase.GUIDToAssetPath(guids[0]);
            }

            if (guids.Length > 1)
            {
                foreach (var guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var filename = Path.GetFileNameWithoutExtension(assetPath);
                    if (filename == type.Name)
                    {
                        return AssetDatabase.GUIDToAssetPath(guid);
                    }
                }
            }

            Debug.LogErrorFormat("Unable to locate {0}", type.Name);
            return null;
        }

#endif
    }
}