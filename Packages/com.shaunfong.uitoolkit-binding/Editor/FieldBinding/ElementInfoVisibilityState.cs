using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.shaunfong.UIToolkitFieldBinding.editor
{
    [Flags]
    public enum ElementInfoVisibilityState
    {
        Name = 1 << 0,
        Type = 1 << 1,
        HideWithoutName = 1 << 2,
        HideSelected = 1 << 3,
        HideNotSelected = 1 << 4,
        All = ~0
    }
}