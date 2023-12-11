#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Layouts
{
    [Serializable]
    public class EditorLayoutInfo
    {
        public VisualTreeAsset UxmlReference;

        public EditorLayoutInfo()
        {
            UxmlReference = null;
        }
    }
}
#endif