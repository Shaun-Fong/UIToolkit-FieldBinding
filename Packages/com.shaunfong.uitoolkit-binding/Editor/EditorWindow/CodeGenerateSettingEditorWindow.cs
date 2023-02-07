using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using com.shaunfong.UIToolkitFieldBinding.editor;
using UnityEditor.UIElements;
using UnityEditor.PackageManager.UI;
using System;
using System.IO;
using System.Net.Http;

public class CodeGenerateSettingEditorWindow : EditorWindow
{

    public string m_NameSpace;

    public string m_ScriptPath;

    public string m_ScriptContent;

    private FieldBindingData m_Data;

    public static CodeGenerateSettingEditorWindow ShowWindow(FieldBindingData data)
    {
        CodeGenerateSettingEditorWindow window = CreateInstance<CodeGenerateSettingEditorWindow>();
        window.titleContent = new GUIContent("Code Generation");
        window.Init(data);
        window.position = new Rect(
            (Screen.currentResolution.width - 350) / 2,
            (Screen.currentResolution.height - 550) / 2,
            350,
            550
            );
        window.ShowModalUtility();
        return window;
    }

    private void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        string vtaPath = PathUtility.GetUXMLRes("CodeGenerationSettingDocument");
        var m_InspectorVisualTreeAsset = (AssetDatabase.LoadAssetAtPath(vtaPath, typeof(VisualTreeAsset)) as VisualTreeAsset);
        root.Add(m_InspectorVisualTreeAsset.Instantiate());

        // Read NameSpace In Prefs.
        m_NameSpace = SettingUtility.GetNameSpaceValue();

        // Register Callbacks
        TextField namespaceField = root.Q<TextField>("namespace");
        namespaceField.RegisterValueChangedCallback<string>(OnNamespaceValueChange);

        Button changeLocation = root.Q<Button>("chaneglocation");
        changeLocation.clicked += OnChangeScriptLocation;

        Button generate = root.Q<Button>("generate");
        generate.clicked += GenerateScript;

        root.Bind(new SerializedObject(this));

        // Refresh Preview.
        RefreshScriptContent();
    }

    private void OnNamespaceValueChange(ChangeEvent<string> evt)
    {
        SettingUtility.SetNameSpaceValue(evt.newValue);
        RefreshScriptContent();
    }

    private void OnChangeScriptLocation()
    {
        var path = OpenSaveFilePanel();

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        m_ScriptPath = path.GetRelativePath();
        m_Data.ScriptPath = m_ScriptPath;

        RefreshScriptContent();

        AssetDatabase.SaveAssets();
    }

    private string OpenSaveFilePanel()
    {
        var path = EditorUtility.SaveFilePanel("Script Path", Application.dataPath, "Script", "cs");

        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        return path;
    }

    private void RefreshScriptContent()
    {
        m_ScriptContent = ScriptGenerator.GetScriptPreview(m_Data, m_NameSpace, Path.GetFileNameWithoutExtension(m_ScriptPath));
    }

    public void Init(FieldBindingData data)
    {
        m_Data = data;
        m_ScriptPath = data.ScriptPath;

        while (string.IsNullOrEmpty(data.ScriptPath))
        {
            OnChangeScriptLocation();
        }
    }

    public void GenerateScript()
    {
        RefreshScriptContent();

        string fileName = Path.GetFileNameWithoutExtension(m_ScriptPath.GetGlobalPath());
        string path = Path.Combine(Path.GetDirectoryName(m_ScriptPath.GetGlobalPath()), fileName + "_BindFields.cs");

        ScriptGenerator.GenerateScript(m_ScriptContent, path);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        Close();
    }
}
