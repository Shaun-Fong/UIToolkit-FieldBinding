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
    [CustomEditor(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public class UIDocumentInspector : UnityEditor.Editor
    {

        private VisualTreeAsset m_TargetVisualTreeAsset;

        private VisualElement rootVisualElement;
        private VisualElement fieldBindingVisualElement;

        private FieldBinding m_FieldBinding;

        public override VisualElement CreateInspectorGUI()
        {
            UIDocument document = (UIDocument)target;

            rootVisualElement = new VisualElement();
            fieldBindingVisualElement = new VisualElement();
            rootVisualElement.Add(fieldBindingVisualElement);

            m_FieldBinding = new FieldBinding(fieldBindingVisualElement);
            m_FieldBinding.LoadFieldsData(document.visualTreeAsset);
            m_FieldBinding.RegisterCallbacks();

            DrawDefaultProperties(rootVisualElement);

            m_FieldBinding.RefreshFieldsList();

            return rootVisualElement;
        }

        private void SerializedPropertyChangeCallback(SerializedPropertyChangeEvent evt)
        {
            UIDocument document = (UIDocument)target;
            m_TargetVisualTreeAsset = document.visualTreeAsset;
            m_FieldBinding.LoadFieldsData(m_TargetVisualTreeAsset);
            m_FieldBinding.RefreshFieldsList();
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