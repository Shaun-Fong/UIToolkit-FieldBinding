using Codice.CM.Common.Serialization.Replication;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
}