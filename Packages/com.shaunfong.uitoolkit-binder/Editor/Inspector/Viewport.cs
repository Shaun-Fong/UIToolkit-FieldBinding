using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.shaunfong.UIToolkitFieldBinding.editor
{

    public class Viewport
    {


        public ElementInfoVisibilityState m_ElementInfoVisibillityState = ElementInfoVisibilityState.All ^ ElementInfoVisibilityState.HideWithoutName;

        private List<FieldSelection> m_LoadedFields = new List<FieldSelection>();
        private List<FieldSelection> m_DisplayFields = new List<FieldSelection>();

        private VisualElement m_FieldBindingVisualElement;
        private string m_SearchString;
        private ListView m_FieldsView;

        private VisualTreeAsset m_TargetVisualTreeAsset;

        public Viewport(VisualElement root)
        {
            m_FieldBindingVisualElement = root;
            string vtaPath = PathUtility.GetUXMLRes("CustomInspectorPanel");
            var m_InspectorVisualTreeAsset = (AssetDatabase.LoadAssetAtPath(vtaPath, typeof(VisualTreeAsset)) as VisualTreeAsset);
            m_InspectorVisualTreeAsset.CloneTree(root);
        }

        internal void LoadAllFields(VisualTreeAsset targetVisualTreeAsset)
        {
            m_TargetVisualTreeAsset = targetVisualTreeAsset;
            if (targetVisualTreeAsset == null)
            {
                m_LoadedFields.Clear();
                return;
            }

            var root = targetVisualTreeAsset.CloneTree();
            var elements = root.GetAllVisualElements(true);

            m_LoadedFields.Clear();
            foreach (var element in elements)
            {
                if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideWithoutName) && string.IsNullOrEmpty(element.name))
                {
                    continue;
                }
                m_LoadedFields.Add(new FieldSelection()
                {
                    FieldName = element.name,
                    FieldType = element.GetType().Name,
                    FieldSelected = string.IsNullOrEmpty(element.name) ? false : true
                });
            }
        }

        internal void RefreshFields()
        {
            GroupBox enable = m_FieldBindingVisualElement.Q<GroupBox>("Enable");
            GroupBox disable = m_FieldBindingVisualElement.Q<GroupBox>("Disable");

            enable.style.display = m_TargetVisualTreeAsset == null ? DisplayStyle.None : DisplayStyle.Flex;
            disable.style.display = m_TargetVisualTreeAsset == null ? DisplayStyle.Flex : DisplayStyle.None;

            if (m_TargetVisualTreeAsset == null)
            {
                return;
            }

            m_DisplayFields.Clear();
            for (int i = 0; i < m_LoadedFields.Count; i++)
            {
                if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideWithoutName) && string.IsNullOrEmpty(m_LoadedFields[i].FieldName))
                    continue;

                if (string.IsNullOrEmpty(m_SearchString) == false && m_LoadedFields[i].FieldName.Contains(m_SearchString) == false)
                    continue;

                m_DisplayFields.Add(m_LoadedFields[i]);
            }
            m_FieldsView.Rebuild();
        }

        internal void RegisterCallbacks()
        {
            ToolbarMenu m_menu = m_FieldBindingVisualElement.Q<ToolbarMenu>("menu");
            m_menu.menu.AppendAction("Select All", a => { SelectAll(); });
            m_menu.menu.AppendAction("Select Reverse", a => { SelectReverse(); });
            m_menu.menu.AppendAction("Select WithName", a => { SelectWithName(); });
            m_menu.menu.AppendAction("Generate", a => { Generate(); });

            ToolbarMenu m_FilterMenu = m_FieldBindingVisualElement.Q<ToolbarMenu>("filter");

            m_FilterMenu.style.display = DisplayStyle.Flex;

            m_FilterMenu.menu.AppendAction("Name",
                a => ChangeVisibilityState(ElementInfoVisibilityState.Name),
                a => m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.Name) ?
                DropdownMenuAction.Status.Checked :
                DropdownMenuAction.Status.Normal
                );
            m_FilterMenu.menu.AppendAction("Type",
                a => ChangeVisibilityState(ElementInfoVisibilityState.Type),
                a => m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.Type) ?
                DropdownMenuAction.Status.Checked :
                DropdownMenuAction.Status.Normal
                );
            m_FilterMenu.menu.AppendAction("HideWithoutName",
                a => ChangeVisibilityState(ElementInfoVisibilityState.HideWithoutName),
                a => m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideWithoutName) ?
                DropdownMenuAction.Status.Checked :
                DropdownMenuAction.Status.Normal
                );

            ToolbarSearchField toolbarSearchField = m_FieldBindingVisualElement.Q<ToolbarSearchField>("search");
            toolbarSearchField.RegisterValueChangedCallback<string>(OnSearchFieldChange);

            m_FieldsView = m_FieldBindingVisualElement.Q<ListView>("fields-view");
            m_FieldsView.makeItem = () =>
            {
                VisualElement container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;

                Toggle toggle = new Toggle();
                toggle.text = "  ";
                toggle.name = "field-toggle";

                Label name = new Label();
                name.name = "field-name";

                Label type = new Label();
                type.name = "field-type";

                container.Add(toggle);
                container.Add(name);
                container.Add(type);

                return container;
            };
            m_FieldsView.bindItem = (e, i) =>
            {
                Toggle toggle = e.Q<Toggle>("field-toggle");
                Label name = e.Q<Label>("field-name");
                Label type = e.Q<Label>("field-type");

                if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.Name | ElementInfoVisibilityState.Type))
                {
                    name.text = $"<color=#93B3F8>{m_DisplayFields[i].FieldName}</color>";
                    type.text = $"<color=#AAAA5B>{m_DisplayFields[i].FieldType}</color>";
                }
                else if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.Name))
                {
                    name.text = $"<color=#93B3F8>{m_DisplayFields[i].FieldName}</color>";
                }
                else if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.Type))
                {
                    type.text = $"<color=#AAAA5B>{m_DisplayFields[i].FieldType}</color>";
                }

                toggle.RegisterValueChangedCallback<bool>((evt) =>
                {
                    m_DisplayFields[i].FieldSelected = evt.newValue;
                });
                toggle.value = m_DisplayFields[i].FieldSelected;
            };
            m_FieldsView.itemsSource = m_DisplayFields;
        }



        private void OnSearchFieldChange(ChangeEvent<string> evt)
        {
            m_SearchString = evt.newValue;
            RefreshFields();
        }

        private void Generate()
        {

        }

        private void SelectWithName()
        {
            foreach (var field in m_DisplayFields)
            {
                field.FieldSelected = string.IsNullOrEmpty(field.FieldName) ? false : true;
            }
            RefreshFields();
        }

        private void SelectReverse()
        {
            foreach (var field in m_DisplayFields)
            {
                field.FieldSelected = !field.FieldSelected;
            }
            RefreshFields();
        }

        private void SelectAll()
        {
            foreach (var field in m_DisplayFields)
            {
                field.FieldSelected = true;
            }
            RefreshFields();
        }

        private void ChangeVisibilityState(ElementInfoVisibilityState state)
        {
            m_ElementInfoVisibillityState ^= state;
            RefreshFields();
        }
    }
}