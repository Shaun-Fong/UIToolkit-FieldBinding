using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.shaunfong.UIToolkitFieldBinding.editor
{
    [System.Serializable]
    public class FieldBindingDataManager : ScriptableObject
    {

        [SerializeField]
        public List<FieldBindingData> Data = new List<FieldBindingData>();

        public bool Exist(string AssetPath)
        {
            return Data.FindAll((x) => x.AssetPath == AssetPath).Count != 0;
        }

        public void Add(FieldBindingData data)
        {
            if (Exist(data.AssetPath) == false)
            {
                Data.Add(data);
            }
        }

        public bool Remove(string AssetPath)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].AssetPath == AssetPath)
                {
                    Data.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        internal FieldBindingData GetFieldBindingData(string visualTreeAssetPath)
        {
            return Data.Find((x) => x.AssetPath == visualTreeAssetPath);
        }

        public static FieldBindingDataManager GetInstance()
        {

            if (ScriptableObjectUtility.CheckExist() == false)
            {
                ScriptableObjectUtility.CreateNewOne();
            }

            return ScriptableObjectUtility.LoadFromAsset();

        }

        internal void Save(FieldBinding fieldBinding)
        {
            if (Exist(fieldBinding.VisualTreeAssetPath) == false)
            {
                SaveAs(fieldBinding);
                return;
            }

            // Save Field Data To Disk.
            FieldBindingData data = GetFieldBindingData(fieldBinding.VisualTreeAssetPath);
            data.FieldDatas.Clear();
            foreach (var field in fieldBinding.DisplayFields)
            {
                data.FieldDatas.Add(field);
            }

            EditorUtility.SetDirty(this);
            ScriptableObjectUtility.SaveAsset(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal void SaveAs(FieldBinding fieldBinding)
        {
            //Remove Exist Data.
            Remove(fieldBinding.VisualTreeAssetPath);

            FieldBindingData data = new FieldBindingData(fieldBinding.VisualTreeAssetPath);

            //Save Field Data To Disk.
            foreach (var field in fieldBinding.DisplayFields)
            {
                data.FieldDatas.Add(field);
            }
            Add(data);

            // Open Code Generation Window.
            CodeGenerateSettingEditorWindow.ShowWindow(data);

            EditorUtility.SetDirty(this);
            ScriptableObjectUtility.SaveAsset(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal void Generate(FieldBinding fieldBinding)
        {
            FieldBindingData data = GetFieldBindingData(fieldBinding.VisualTreeAssetPath);

            if (data == null || File.Exists(data.ScriptPath.GetGlobalPath()) == false)
            {
                SaveAs(fieldBinding);
                return;
            }

            //Save Field Data To Disk.
            data.FieldDatas.Clear();
            foreach (var field in fieldBinding.DisplayFields)
            {
                data.FieldDatas.Add(field);
            }

            string fileName = Path.GetFileNameWithoutExtension(data.ScriptPath);
            string path = Path.Combine(Path.GetDirectoryName(data.ScriptPath.GetGlobalPath()), fileName + "_BindFields.cs");

            string content = ScriptGenerator.GetScriptPreview(data, SettingUtility.GetNameSpaceValue(), SettingUtility.GetInheritClassValue(), SettingUtility.GetAccessModifierValue(), fileName);

            ScriptGenerator.GenerateScript(content, path);

            EditorUtility.SetDirty(this);
            ScriptableObjectUtility.SaveAsset(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
