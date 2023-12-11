#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Enums;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Fonts
{
    [Serializable]
    public class EditorFontInfo
    {
        public Font FontReference;
        public GenericFontWeight Weight;

        public EditorFontInfo()
        {
            FontReference = null;
            Weight = GenericFontWeight.Regular;
        }
    }
}
#endif
