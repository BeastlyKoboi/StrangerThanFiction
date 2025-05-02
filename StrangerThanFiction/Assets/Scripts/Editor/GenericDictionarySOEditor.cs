using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(ScriptableObject), true)]
public class GenericDictionarySOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var so = target as ScriptableObject;
        var type = so.GetType();

        // Check if it's a GenericDictionarySO<>
        var baseType = type.BaseType;
        if (baseType == null || !baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(GenericDictionarySO<,>))
            return;

        if (GUILayout.Button("Auto Populate Dictionary"))
        {
            var keyType = baseType.GetGenericArguments()[0];
            var valueType = baseType.GetGenericArguments()[1];

            // Only proceed if valueType is a ScriptableObject
            if (!typeof(ScriptableObject).IsAssignableFrom(valueType))
            {
                Debug.LogWarning($"Value type {valueType.Name} is not a ScriptableObject. Cannot auto-populate.");
                return;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{valueType.Name}");
            var clearMethod = baseType.GetMethod("ClearEntries");
            var addMethod = baseType.GetMethod("AddEntry");

            clearMethod.Invoke(so, null);

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, valueType) as ScriptableObject;

                if (asset == null) continue;

                string key = asset.name;
                object typedKey = Convert.ChangeType(key, keyType);

                addMethod.Invoke(so, new object[] { typedKey, asset });
            }

            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            Debug.Log($"{valueType.Name} dictionary updated successfully!");
        }
    }
}