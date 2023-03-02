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
        private List<FieldBindingData> Data = new List<FieldBindingData>();

        public bool Exist(int id)
        {
            return Data.FindAll((x) => x.Id == id).Count != 0;
        }

        public void Add(FieldBindingData data)
        {
            if (Exist(data.Id) == false)
            {
                Data.Add(data);
            }
        }

        public bool Remove(int id)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Id == id)
                {
                    Data.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public FieldBindingData GetFieldBindingData(int id)
        {
            return Data.Find((x) => x.Id == id); ;
        }

        public static FieldBindingDataManager GetInstance()
        {
            var assetIDs = AssetDatabase.FindAssets("t:FieldBindingDataManager");

            if (assetIDs.Length != 0)
            {
                if (assetIDs.Length > 1)
                {
                    Debug.LogWarning("More than 1 Asset founded, first one will be using.");
                }
                return AssetDatabase.LoadAssetAtPath<FieldBindingDataManager>(AssetDatabase.GUIDToAssetPath(assetIDs[0]));
            }

            FieldBindingDataManager dataManager = ScriptableObject.CreateInstance<FieldBindingDataManager>();

            string filePath = Path.Combine(Application.dataPath, "Editor", "FieldBindingData.asset");

            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }

            Debug.LogWarning($"Field Binding Data Asset Create In '{filePath}'.");

            AssetDatabase.CreateAsset(dataManager, filePath.Replace(Application.dataPath, "Assets"));
            AssetDatabase.SaveAssets();


            return dataManager;

        }

        internal void Save(FieldBinding fieldBinding)
        {
            if (Exist(fieldBinding.ID) == false)
            {
                SaveAs(fieldBinding);
                return;
            }

            // Save Field Data To Disk.
            FieldBindingData data = GetFieldBindingData(fieldBinding.ID);
            data.FieldDatas.Clear();
            foreach (var field in fieldBinding.DisplayFields)
            {
                data.FieldDatas.Add(field);
            }

            AssetDatabase.SaveAssets();
        }

        internal void SaveAs(FieldBinding fieldBinding)
        {
            //Remove Exist Data.
            Remove(fieldBinding.ID);

            FieldBindingData data = new FieldBindingData(fieldBinding.ID);

            //Save Field Data To Disk.
            foreach (var field in fieldBinding.DisplayFields)
            {
                data.FieldDatas.Add(field);
            }
            Add(data);

            // Open Code Generation Window.
            CodeGenerateSettingEditorWindow.ShowWindow(data);

            AssetDatabase.SaveAssets();
        }

        internal void Generate(FieldBinding fieldBinding)
        {
            FieldBindingData data = GetFieldBindingData(fieldBinding.ID);

            if (data == null)
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

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
