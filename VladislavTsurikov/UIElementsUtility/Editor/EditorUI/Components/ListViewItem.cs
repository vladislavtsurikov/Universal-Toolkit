#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Components
{
    public abstract class ListViewItem : VisualElement
    {
        //REFERENCES
        public TemplateContainer TemplateContainer { get; }
        public VisualElement LayoutContainer { get; }
        public Label ItemIndexLabel { get; }
        public VisualElement ItemContentContainer { get; }
        public VisualElement ItemRemoveButtonContainer { get; }

        //SETTINGS
        public ListView ListView { get; internal set; }
        public int ItemIndex { get; protected set; }
        public Button ItemRemoveButton { get; }

        public bool ShowItemIndex { get; protected set; }
        public bool ShowRemoveButton { get; protected set; }

        private static Color ItemIndexTextColor => EditorColors.Default.TextDescription;

        private static Font IndexLabelFont => EditorFonts.Ubuntu.Light;

        protected ListViewItem()
        {
            this.SetStyleJustifyContent(Justify.Center);
            
            Add(TemplateContainer = EditorLayouts.EditorUI.ListViewItem.CloneTree());
            TemplateContainer
                .AddStyle(EditorStyles.EditorUI.ListViewItem);
            
            //REFERENCES
            LayoutContainer = TemplateContainer.Q<VisualElement>("LayoutContainer");
            ItemIndexLabel = LayoutContainer.Q<Label>("ItemIndex");
            ItemContentContainer = LayoutContainer.Q<VisualElement>("ItemContentContainer");
            ItemRemoveButtonContainer = LayoutContainer.Q<VisualElement>("ItemRemoveButtonContainer");

            //TEXT COLORS
            ItemIndexLabel.SetStyleColor(ItemIndexTextColor);
            
            //TEXT FONTS
            ItemIndexLabel.SetStyleUnityFont(IndexLabelFont);
        }
        
        protected ListViewItem(ListView listView) : this()
        {
            this.SetListView(listView);
            ItemRemoveButtonContainer.Clear();
            ItemRemoveButton = ListView.Buttons.RemoveButton;
            ItemRemoveButtonContainer.Add(ItemRemoveButton);
        }

        internal virtual void UpdateItemIndex(int value)
        {
            ItemIndexLabel.SetStyleDisplay(ShowItemIndex ? DisplayStyle.Flex : DisplayStyle.None);
            value = Mathf.Max(0, value);
            ItemIndex = value;
            ItemIndexLabel.text = value.ToString();
        }
    }

    public static class FluidListViewItemExtensions
    {
        internal static T SetListView<T>(this T target, ListView value) where T : ListViewItem
        {
            target.ListView = value;
            return target;
        }
        
        public static T SetItemIndex<T>(this T target, int value) where T : ListViewItem
        {
            target.UpdateItemIndex(value);
            return target;
        }

        public static T EnableItemRemoveButton<T>(this T target) where T : ListViewItem
        {
            target.ItemRemoveButton.EnableElement();
            return target;
        }

        public static T DisableItemRemoveButton<T>(this T target) where T : ListViewItem
        {
            target.ItemRemoveButton.DisableElement();
            return target;
        }

        public static T ToggleItemRemoveButton<T>(this T target, bool enabled) where T : ListViewItem
        {
            return enabled
                ? target.EnableItemRemoveButton()
                : target.DisableItemRemoveButton();
        }
    }
}
#endif