using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.shaunfong.UIToolkitFieldBinding.editor
{
    [System.Serializable]
    public class FieldBindingData
    {
        public string AssetPath;
        public string ScriptPath;
        public List<FieldSelection> FieldDatas = new List<FieldSelection>();

        public FieldBindingData(string assetPath)
        {
            AssetPath = assetPath;
        }
    }
}