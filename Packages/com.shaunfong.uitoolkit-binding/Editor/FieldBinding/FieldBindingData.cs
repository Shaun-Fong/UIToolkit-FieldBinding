using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.shaunfong.UIToolkitFieldBinding.editor
{
    [System.Serializable]
    public class FieldBindingData
    {
        public int Id;
        public string ScriptPath;
        public List<FieldSelection> FieldDatas = new List<FieldSelection>();

        public FieldBindingData(int id)
        {
            Id = id;
        }
    }
}