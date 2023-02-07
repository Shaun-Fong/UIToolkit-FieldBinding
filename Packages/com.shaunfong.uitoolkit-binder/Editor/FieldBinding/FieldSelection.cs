using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.shaunfong.UIToolkitFieldBinding.editor
{
    [System.Serializable]
    public class FieldSelection
    {
        public string FieldName;
        public string FieldType;
        public bool FieldSelected;
    }

    public class FieldSelectionVisualElement : VisualElement
    {
        public FieldSelectionVisualElement()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;

            Toggle toggle = new Toggle() { name = "FieldSelected" };
            toggle.text = "  ";

            Label name = new Label() { name = "FieldName" };

            Label type = new Label() { name = "FieldType" };

            root.Add(toggle);
            root.Add(name);
            root.Add(type);

            Add(root);
        }
    }
}
