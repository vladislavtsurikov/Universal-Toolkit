#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Components
{
    public sealed class VisualElementListViewItem : ListViewItem
    {
        public VisualElementListViewItem(VisualElement visualElement, int index, ListView listView) : base(listView)
        {
            ShowItemIndex = true;
            ItemContentContainer.Clear();
            ItemContentContainer.Add(visualElement);
            UpdateItemIndex(index);
        }
    }
}
#endif