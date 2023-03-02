using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.shaunfong.UIToolkitFieldBinding.editor
{
    internal static class VisualElementUtility
    {
        public static List<VisualElement> GetAllVisualElements(this VisualElement visualElement, bool childInclude = false)
        {
            List<VisualElement> list = new List<VisualElement>();
            GetChildVisualElements(visualElement, list, childInclude);
            return list;
        }

        private static void GetChildVisualElements(VisualElement root, List<VisualElement> list, bool childInclude)
        {
            var rootEnumerator = root.Children().GetEnumerator();

            while (rootEnumerator.MoveNext())
            {
                list.Add(rootEnumerator.Current);

                if (rootEnumerator.Current.childCount != 0 && childInclude)
                {
                    GetChildVisualElements(rootEnumerator.Current, list, childInclude);
                }
            }
        }
    }

    internal static class PathUtility
    {
        public static string GetPackagesRes(string filename)
        {
            return Path.Combine("Packages", "com.shaunfong.uitoolkit-fieldbinding", "Editor", "Res", filename);
        }

        public static string GetUXMLRes(string uxml)
        {
            return GetPackagesRes(uxml + ".uxml");
        }
    }

    internal static class FieldUtility
    {
        public static bool ValidFieldName(string fieldName)
        {
            Match result = Regex.Match(fieldName, "^[a-zA-Z_][a-zA-Z0-9_]*$");
            return result.Success;
        }
    }

    internal static class StringUtility
    {
        public static string GetRelativePath(this string path)
        {
            return path.Replace(Application.dataPath + "/", "Assets/").Replace("\\", "/");
        }

        public static string GetGlobalPath(this string path)
        {
            return (Application.dataPath.Replace("Assets", "") + path).Replace("\\", "/");
        }

        public static string GetTypeFullName(this Type type)
        {
            if (type.GetTypeInfo().DeclaringType == null)
            {
                return type.GetTypeInfo().FullName;
            }
            else
            {
                return GetTypeFullName(type.GetTypeInfo().DeclaringType) + "." + type.Name;
            }
        }
    }

    internal static class SettingUtility
    {
        internal static string GetNameSpaceValue()
        {
            return EditorPrefs.GetString(Application.companyName + "." + Application.productName + ".FieldBindingSetting.NameSpace", "");
        }

        internal static void SetNameSpaceValue(string value)
        {
            EditorPrefs.SetString(Application.companyName + "." + Application.productName + ".FieldBindingSetting.NameSpace", value);
        }

        internal static string GetInheritClassValue()
        {
            return EditorPrefs.GetString(Application.companyName + "." + Application.productName + ".FieldBindingSetting.InheritClass", "MonoBehaviour");
        }

        public static void SetInheritClassValue(string value)
        {
            EditorPrefs.SetString(Application.companyName + "." + Application.productName + ".FieldBindingSetting.InheritClass", value);
        }

        internal static AccessModifiers GetAccessModifierValue()
        {
            return (AccessModifiers)EditorPrefs.GetInt(Application.companyName + "." + Application.productName + ".FieldBindingSetting.AccessModifier", 1 << 0);
        }

        internal static void SetAccessModifierValue(AccessModifiers value)
        {
            EditorPrefs.SetInt(Application.companyName + "." + Application.productName + ".FieldBindingSetting.AccessModifier", (int)value);
        }


        internal static int GetVisibillityStateValue()
        {
            int defaultValue = (int)(ElementInfoVisibilityState.All ^ ElementInfoVisibilityState.HideWithoutName ^ ElementInfoVisibilityState.HideSelected ^ ElementInfoVisibilityState.HideNotSelected);
            return EditorPrefs.GetInt(Application.companyName + "." + Application.productName + ".FieldBindingSetting.VisibillityState", defaultValue);
        }

        internal static void SetVisibillityState(int state)
        {
            EditorPrefs.SetInt(Application.companyName + "." + Application.productName + ".FieldBindingSetting.VisibillityState", state);
        }
    }
}