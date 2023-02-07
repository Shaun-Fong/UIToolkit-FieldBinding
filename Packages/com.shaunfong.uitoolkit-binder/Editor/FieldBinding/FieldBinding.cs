using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

namespace com.shaunfong.UIToolkitFieldBinding.editor
{

    public class FieldBinding
    {
        public int ID { get; private set; }
        public ElementInfoVisibilityState m_ElementInfoVisibillityState = ElementInfoVisibilityState.All ^ ElementInfoVisibilityState.HideWithoutName ^ ElementInfoVisibilityState.HideSelected ^ ElementInfoVisibilityState.HideNotSelected;
        private List<FieldSelection> m_LoadedFieldData = new List<FieldSelection>();
        public List<FieldSelection> DisplayFields { get; private set; } = new List<FieldSelection>();

        private ListView m_FieldsView;
        private string m_SearchString;
        private VisualElement m_FieldBindingVisualElement;
        private VisualTreeAsset m_TargetVisualTreeAsset;
        private FieldBindingDataManager m_FieldBindingDataManager;

        public FieldBinding(int id, VisualElement root)
        {
            ID = id;
            m_FieldBindingVisualElement = root;

            //Load VisibilityState Setting
            m_ElementInfoVisibillityState = (ElementInfoVisibilityState)SettingUtility.GetVisibillityStateValue();

            //Load UXML
            string vtaPath = PathUtility.GetUXMLRes("FieldBindingDocument");
            var m_InspectorVisualTreeAsset = (AssetDatabase.LoadAssetAtPath(vtaPath, typeof(VisualTreeAsset)) as VisualTreeAsset);
            m_InspectorVisualTreeAsset.CloneTree(root);

            //Get Tracking Data From ScriptableObject
            m_FieldBindingDataManager = FieldBindingDataManager.GetInstance();
        }

        internal void LoadFieldsData(VisualTreeAsset targetVisualTreeAsset)
        {
            m_TargetVisualTreeAsset = targetVisualTreeAsset;

            if (targetVisualTreeAsset == null)
            {
                m_LoadedFieldData.Clear();
                return;
            }

            var root = targetVisualTreeAsset.CloneTree();
            var elements = root.GetAllVisualElements(true);

            m_LoadedFieldData.Clear();
            foreach (var element in elements)
            {
                if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideWithoutName) && string.IsNullOrEmpty(element.name))
                {
                    continue;
                }
                m_LoadedFieldData.Add(new FieldSelection()
                {
                    FieldName = element.name,
                    FieldType = element.GetType().Name,
                    FieldSelected = (string.IsNullOrEmpty(element.name) || FieldUtility.ValidFieldName(element.name) == false) ? false : true
                });
            }
        }

        internal void RefreshFieldsList(bool force = false)
        {
            //Assign Alert
            GroupBox enable = m_FieldBindingVisualElement.Q<GroupBox>("Enable");
            GroupBox disable = m_FieldBindingVisualElement.Q<GroupBox>("Disable");

            enable.style.display = m_TargetVisualTreeAsset == null ? DisplayStyle.None : DisplayStyle.Flex;
            disable.style.display = m_TargetVisualTreeAsset == null ? DisplayStyle.Flex : DisplayStyle.None;

            if (m_TargetVisualTreeAsset == null)
            {
                return;
            }

            DisplayFields.Clear();
            FieldBindingData storageBindingData = m_FieldBindingDataManager.GetFieldBindingData(ID);
            for (int i = 0; i < m_LoadedFieldData.Count; i++)
            {
                if (string.IsNullOrEmpty(m_SearchString) == false && m_LoadedFieldData[i].FieldName.Contains(m_SearchString) == false && m_LoadedFieldData[i].FieldType.Contains(m_SearchString) == false)
                    continue;

                if (force == false && storageBindingData != null)
                {
                    foreach (var fieldSelection in storageBindingData.FieldDatas)
                    {
                        if (m_LoadedFieldData[i].FieldName == fieldSelection.FieldName && m_LoadedFieldData[i].FieldType == fieldSelection.FieldType)
                        {
                            m_LoadedFieldData[i].FieldSelected = fieldSelection.FieldSelected;
                            break;
                        }
                    }
                }

                //Filter
                if ((m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideWithoutName) && string.IsNullOrEmpty(m_LoadedFieldData[i].FieldName)) ||
                    (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideSelected) && m_LoadedFieldData[i].FieldSelected) ||
                    (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideNotSelected) && !m_LoadedFieldData[i].FieldSelected))
                {
                    continue;
                }
                DisplayFields.Add(m_LoadedFieldData[i]);
            }

            m_FieldsView.Rebuild();
        }

        internal void RegisterCallbacks()
        {
            ToolbarMenu m_menu = m_FieldBindingVisualElement.Q<ToolbarMenu>("menu");
            m_menu.menu.AppendAction("Generate", a => { Popup_Generate(); });
            m_menu.menu.AppendAction("Save", a => { Popup_Save(); });
            m_menu.menu.AppendAction("SaveAs", a => { Popup_SaveAs(); });
            m_menu.menu.AppendAction("Select/ValidName", a => { Popup_SelectValid(); });
            m_menu.menu.AppendAction("Select/NameNotNull", a => { Popup_SelectWithName(); });
            m_menu.menu.AppendAction("Select/Reverse", a => { Popup_SelectReverse(); });
            m_menu.menu.AppendAction("Select/All", a => { Popup_SelectAll(); });

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
            m_FilterMenu.menu.AppendAction("HideSelected",
                a => ChangeVisibilityState(ElementInfoVisibilityState.HideSelected),
                a => m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideSelected) ?
                DropdownMenuAction.Status.Checked :
                DropdownMenuAction.Status.Normal
                );
            m_FilterMenu.menu.AppendAction("HideNotSelected",
                a => ChangeVisibilityState(ElementInfoVisibilityState.HideNotSelected),
                a => m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.HideNotSelected) ?
                DropdownMenuAction.Status.Checked :
                DropdownMenuAction.Status.Normal
                );

            ToolbarSearchField toolbarSearchField = m_FieldBindingVisualElement.Q<ToolbarSearchField>("search");
            toolbarSearchField.RegisterValueChangedCallback<string>(OnSearchFieldChange);

            m_FieldsView = m_FieldBindingVisualElement.Q<ListView>("fields-view");

            Func<VisualElement> MakeItem = () =>
            {
                FieldSelectionVisualElement visualElement = new FieldSelectionVisualElement();

                Toggle toggle = visualElement.Q<Toggle>("FieldSelected");

                toggle.RegisterValueChangedCallback<bool>((evt) =>
                {
                    var i = (int)toggle.userData;
                    FieldSelection data = DisplayFields[i];
                    data.FieldSelected = evt.newValue;
                });

                toggle.RegisterCallback<ClickEvent>((evt) =>
                {
                    var i = (int)toggle.userData;
                    FieldSelection data = DisplayFields[i];
                    if (data.FieldSelected == true && FieldUtility.ValidFieldName(data.FieldName) == false)
                    {
                        Debug.LogWarning($"Field Name '{data.FieldName}' Is Invalid , if you still want to continue generating, an error will be reported.");
                    }
                });

                return visualElement;
            };


            Action<VisualElement, int> bindItem = (e, i) =>
            {
                FieldSelectionVisualElement visualElement = e as FieldSelectionVisualElement;
                BindItem(visualElement, i);
            };

            m_FieldsView.makeItem = MakeItem;
            m_FieldsView.bindItem = bindItem;
            m_FieldsView.itemsSource = DisplayFields;
        }

        private void BindItem(FieldSelectionVisualElement e, int i)
        {
            FieldSelection data = DisplayFields[i];

            Toggle toggle = e.Q<Toggle>("FieldSelected");
            toggle.userData = i;
            Label name = e.Q<Label>("FieldName");
            Label type = e.Q<Label>("FieldType");

            if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.Name | ElementInfoVisibilityState.Type))
            {
                name.text = $"<color=#93B3F8>{data.FieldName}</color>";
                type.text = $"<color=#AAAA5B>{data.FieldType}</color>";
            }
            else if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.Name))
            {
                name.text = $"<color=#93B3F8>{data.FieldName}</color>";
            }
            else if (m_ElementInfoVisibillityState.HasFlag(ElementInfoVisibilityState.Type))
            {
                type.text = $"<color=#AAAA5B>{data.FieldType}</color>";
            }

            toggle.value = data.FieldSelected;
        }

        private void OnSearchFieldChange(ChangeEvent<string> evt)
        {
            m_SearchString = evt.newValue;
            RefreshFieldsList();
        }

        private void ChangeVisibilityState(ElementInfoVisibilityState state)
        {
            m_ElementInfoVisibillityState ^= state;
            SettingUtility.SetVisibillityState((int)m_ElementInfoVisibillityState);
            RefreshFieldsList();
        }

        private void Popup_Generate()
        {
            m_FieldBindingDataManager.Generate(this);
        }

        private void Popup_Save()
        {
            m_FieldBindingDataManager.Save(this);
        }

        private void Popup_SaveAs()
        {
            m_FieldBindingDataManager.SaveAs(this);
        }

        private void Popup_SelectWithName()
        {
            foreach (var field in DisplayFields)
            {
                field.FieldSelected = string.IsNullOrEmpty(field.FieldName) ? false : true;
            }
            RefreshFieldsList(true);
        }

        private void Popup_SelectValid()
        {
            foreach (var field in DisplayFields)
            {
                field.FieldSelected = (string.IsNullOrEmpty(field.FieldName) || FieldUtility.ValidFieldName(field.FieldName) == false) ? false : true;
            }
            RefreshFieldsList(true);
        }

        private void Popup_SelectReverse()
        {
            foreach (var field in DisplayFields)
            {
                field.FieldSelected = !field.FieldSelected;
            }
            RefreshFieldsList(true);
        }

        private void Popup_SelectAll()
        {
            foreach (var field in DisplayFields)
            {
                field.FieldSelected = true;
            }
            RefreshFieldsList(true);
        }
    }
}