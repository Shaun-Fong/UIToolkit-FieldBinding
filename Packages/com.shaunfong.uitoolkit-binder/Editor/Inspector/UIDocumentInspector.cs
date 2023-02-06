using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static com.shaunfong.UIToolkitFieldBinding.editor.UIDocumentInspector;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using Toggle = UnityEngine.UIElements.Toggle;

namespace com.shaunfong.UIToolkitFieldBinding.editor
{
    [Flags]
    public enum ElementInfoVisibilityState
    {
        Name = 1 << 0,
        Type = 1 << 1,
        HideWithoutName = 1 << 2,
        All = ~0
    }

    public class FieldSelection
    {
        public string FieldName;
        public string FieldType;
        public bool FieldSelected;
    }

    [CustomEditor(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public class UIDocumentInspector : UnityEditor.Editor
    {

        private VisualTreeAsset m_TargetVisualTreeAsset;

        private VisualElement rootVisualElement;
        private VisualElement fieldBindingVisualElement;

        private Viewport m_ViewPort;

        public override VisualElement CreateInspectorGUI()
        {

            rootVisualElement = new VisualElement();
            fieldBindingVisualElement = new VisualElement();
            rootVisualElement.Add(fieldBindingVisualElement);

            m_ViewPort = new Viewport(fieldBindingVisualElement);
            m_ViewPort.LoadAllFields(((UIDocument)target).visualTreeAsset);
            m_ViewPort.RegisterCallbacks();

            DrawDefaultProperties(rootVisualElement);

            m_ViewPort.RefreshFields();

            return rootVisualElement;
        }

        private void SerializedPropertyChangeCallback(SerializedPropertyChangeEvent evt)
        {
            UIDocument document = (UIDocument)target;
            if (m_TargetVisualTreeAsset != document.visualTreeAsset)
            {
                m_TargetVisualTreeAsset = document.visualTreeAsset;
                m_ViewPort.LoadAllFields(m_TargetVisualTreeAsset);
                m_ViewPort.RefreshFields();
            }
        }

        private void DrawDefaultProperties(VisualElement root)
        {
            //Draw Default Inspector PropertyFields.
            SerializedProperty iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(enterChildren: true))
            {
                do
                {
                    PropertyField propertyField = new PropertyField(iterator);
                    propertyField.name = "PropertyField:" + iterator.propertyPath;

                    //Skip ParentUI Property.
                    if (iterator.propertyPath == "m_ParentUI" || iterator.propertyPath == "m_Script")
                    {
                        continue;
                    }

                    root.Add(propertyField);
                    propertyField.RegisterValueChangeCallback(SerializedPropertyChangeCallback);
                }
                while (iterator.NextVisible(enterChildren: false));
            }
        }

    }
}