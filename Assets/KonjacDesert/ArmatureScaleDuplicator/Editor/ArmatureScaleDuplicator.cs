using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

/*
 * 
 * ArmatureScaleDuplicator
 * 
 * rev 1
 * 
 * date 20230215
 * 
 * author KonjacDesert
 * 
 */

namespace KonjacDesert
{
    static class ArmatureScaleDuplicator
    {
        static readonly Dictionary<string, Vector3> dictionary = new Dictionary<string, Vector3>();

        internal static Transform GetArmature()
        {
            if (Selection.activeTransform)
            {
                Transform armature;
                if (Selection.activeTransform.name.ToLower() == "armature")
                {
                    armature = Selection.activeTransform;
                }
                else
                {
                    armature = Selection.activeTransform.Find("Armature");
                }

                if (armature)
                {
                    return armature;
                }
            }
            return null;
        }

        internal static bool Validate()
        {
            return GetArmature() != null;
        }

        [MenuItem("GameObject/ArmatureScale/Copy", validate = true)]
        static bool CopyValidate()
        {
            return Validate();
        }

        [MenuItem("GameObject/ArmatureScale/Paste", validate = true)]
        static bool PasteValidate()
        {
            return Validate();
        }

        [MenuItem("GameObject/ArmatureScale/Copy", priority = 21)]
        static void Copy()
        {
            var armature = GetArmature();
            if (armature)
            {
                dictionary.Clear();

                int count = 0;
                IEnumerable<Transform> children = armature.EnumChildrenRecursive();
                foreach (var item in children)
                {
                    dictionary.Add(GetPath(item, armature), item.localScale);
                    count++;
                }
                Debug.Log($"[{nameof(ArmatureScaleDuplicator)}]{count} Scale saved");
            }
        }

        [MenuItem("GameObject/ArmatureScale/Paste", priority = 22)]
        static void Paste()
        {
            var armature = GetArmature();
            if (armature)
            {
                int count = 0;
                IEnumerable<Transform> children = armature.EnumChildrenRecursive();
                foreach (var item in children)
                {
                    var path = GetPath(item, armature);
                    if (dictionary.ContainsKey(path))
                    {
                        if (item.localScale != dictionary[path])
                        {
                            Undo.RecordObject(item, "Paste Scale");
                            item.localScale = dictionary[path];
                            count++;
                        }
                    }
                }
                Debug.Log($"[{nameof(ArmatureScaleDuplicator)}]{count} Scale matched");
            }
        }

        static string GetPath(Transform child, Transform root)
        {
            string path = child.name;
            Transform current = child;
            while (current != root)
            {
                current = current.parent;
                path = current.name + "/" + path;
            }
            return path;
        }
        static IEnumerable<Transform> EnumChildrenRecursive(this Transform parent)
        {
            return parent.GetComponentsInChildren<Transform>();
        }
    }
}