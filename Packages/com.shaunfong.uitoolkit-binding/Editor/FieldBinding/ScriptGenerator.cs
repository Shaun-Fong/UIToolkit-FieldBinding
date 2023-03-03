using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.shaunfong.UIToolkitFieldBinding.editor
{
    public static class ScriptGenerator
    {
        internal static void GenerateScript(string content, string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
            {
                sw.Write(content);
            }
            Debug.Log($"Generate Script At '{path}'.");
        }

        internal static string GetScriptPreview(FieldBindingData m_Data, string classNameSpace, string inheritClass, AccessModifiers accessModifier, string className)
        {
            string result =
                    "using UnityEngine;" +
                    System.Environment.NewLine +
                    "using UnityEngine.UI;" +
                    System.Environment.NewLine +
                    "using UnityEngine.UIElements;" +
                    System.Environment.NewLine;

            if (string.IsNullOrEmpty(classNameSpace))
            {

                result +=
                    "[RequireComponent(typeof(UnityEngine.UIElements.UIDocument))]" +
                    System.Environment.NewLine +
                    "{ACCESSMODIFIERS} partial class {CLASSNAME} : {INHERITCLASS}" +
                    System.Environment.NewLine +
                    "{" +
                    System.Environment.NewLine +
                    "{CONTENT}" +
                    System.Environment.NewLine +
                    "    public void Bind()" +
                    System.Environment.NewLine +
                    "    {" +
                    System.Environment.NewLine +
                    "        VisualElement root = gameObject.GetComponent<UIDocument>().rootVisualElement;" +
                    System.Environment.NewLine +
                    "{BINDING}" +
                    "    }" +
                    System.Environment.NewLine +
                    "}" +
                    System.Environment.NewLine;
            }
            else
            {

                result +=
                    "namespace {NAMESPACE}" +
                    System.Environment.NewLine +
                    "{" +
                    System.Environment.NewLine +
                    "    [RequireComponent(typeof(UnityEngine.UIElements.UIDocument))]" +
                    System.Environment.NewLine +
                    "    {ACCESSMODIFIERS} partial class {CLASSNAME} : {INHERITCLASS}" +
                    System.Environment.NewLine +
                    "    {" +
                    System.Environment.NewLine +
                    "{CONTENT}" +
                    System.Environment.NewLine +
                    "        public void Bind()" +
                    System.Environment.NewLine +
                    "        {" +
                    System.Environment.NewLine +
                    "            VisualElement root = gameObject.GetComponent<UIDocument>().rootVisualElement;" +
                    System.Environment.NewLine +
                    "{BINDING}" +
                    "        }" +
                    System.Environment.NewLine +
                    "    }" +
                    System.Environment.NewLine +
                    "}" +
                    System.Environment.NewLine;
            }


            string content = "";
            for (int i = 0; i < m_Data.FieldDatas.Count; i++)
            {
                if (m_Data.FieldDatas[i].FieldSelected == false) continue;
                content +=
                    (string.IsNullOrEmpty(classNameSpace) ? "    " : "        ") +
                    $"public {m_Data.FieldDatas[i].FieldType} {m_Data.FieldDatas[i].FieldName}" + " { get; private set; }" +
                    (i == m_Data.FieldDatas.Count - 1 ? "" : System.Environment.NewLine);
            }

            string binding = "";
            for (int i = 0; i < m_Data.FieldDatas.Count; i++)
            {
                if (m_Data.FieldDatas[i].FieldSelected == false) continue;
                binding +=
                    (string.IsNullOrEmpty(classNameSpace) ? "        " : "            ") +
                    $"{m_Data.FieldDatas[i].FieldName} = root.Q<{m_Data.FieldDatas[i].FieldType}>(\"{m_Data.FieldDatas[i].FieldName}\");"+ System.Environment.NewLine;
            }

            string accessModifierStr;

            switch (accessModifier)
            {
                case AccessModifiers.PUBLIC:
                    accessModifierStr = "public";
                    break;
                case AccessModifiers.PRIVATE:
                    accessModifierStr = "private";
                    break;
                case AccessModifiers.PROTECTED:
                    accessModifierStr = "protected";
                    break;
                case AccessModifiers.INTERNAL:
                    accessModifierStr = "internal";
                    break;
                case AccessModifiers.PROTECTED_INTERNAL:
                    accessModifierStr = "protected internal";
                    break;
                case AccessModifiers.PRIVATE_PROTECTED:
                    accessModifierStr = "private protected";
                    break;
                default:
                    accessModifierStr = "public";
                    break;
            }

            result = result.
                Replace("{NAMESPACE}", classNameSpace).
                Replace("{ACCESSMODIFIERS}", accessModifierStr).
                Replace("{CLASSNAME}", className).
                Replace("{INHERITCLASS}", inheritClass).
                Replace("{CONTENT}", content).
                Replace("{BINDING}", binding);

            return result;
        }
    }
}