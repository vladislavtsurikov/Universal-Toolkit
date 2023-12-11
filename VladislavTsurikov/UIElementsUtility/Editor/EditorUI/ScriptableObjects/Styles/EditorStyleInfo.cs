#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Styles
{
    [Serializable]
    public class EditorStyleInfo
    {
        public StyleSheet UssReference;

        public EditorStyleInfo()
        {
            UssReference = null;
        }
    }
}
#endif