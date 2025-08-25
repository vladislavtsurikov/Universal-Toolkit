#if UNITY_EDITOR
#if !DISABLE_VISUAL_ELEMENTS
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility;
using VladislavTsurikov.Utility.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors;
using VladislavTsurikov.UIElementsUtility.Runtime;
using VisualElementExtensions = VladislavTsurikov.UIElementsUtility.Runtime.Utility.VisualElementExtensions;

namespace VladislavTsurikov.SceneDataSystem.Editor.VisualElements
{
    public class ListView : VisualElement
    {
        private const int ListMinimumHeight = 120;
        private List<VisualElement> _listViewItems = new List<VisualElement>();
        
        public UnityAction AddNewItemButtonCallback;

        public TemplateContainer TemplateContainer { get; }
        public VisualElement LayoutContainer { get; }
        public VisualElement HeaderContainer { get; }
        public Label ListTitle { get; }
        public VisualElement ListViewContainer { get; }
        public ScrollView ScrollView;

        public List<VisualElement> ToolbarElements { get; }
        
        
        private Button _mAddItemButton;
        public Button AddItemButton => _mAddItemButton ??= Buttons.AddButton;

        public bool ShowItemIndex { get; private set; }
        public int PreferredListHeight { get; private set; }
        public bool AddNewItemButtonIsHidden { get; private set; }

        //SELECTABLE COLORS
        private static EditorSelectableColorInfo AddSelectableColor => GetSelectableColor.EditorUI.Add;
        private static EditorSelectableColorInfo RemoveSelectableColor => GetSelectableColor.EditorUI.Remove;

        //COLORS
        private static Color ListNameTextColor => GetEditorColor.EditorUI.TextTitle;
        private static Color BackgroundColor => GetEditorColor.EditorUI.Background;

        //FONTS
        private static Font ListNameFont => GetFont.Ubuntu.Light;

        public ListView()
        {
            Add(TemplateContainer = GetLayout.VisualElements.ListView.CloneTree());
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.AddStyle(UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleFlexGrow(TemplateContainer, 1), GetStyle.VisualElements.ListView);

            //REFERENCES
            LayoutContainer = TemplateContainer.Q<VisualElement>(nameof(LayoutContainer));
            HeaderContainer = LayoutContainer.Q<VisualElement>(nameof(HeaderContainer));
            ListTitle = HeaderContainer.Q<Label>(nameof(ListTitle)); 
            ListViewContainer = LayoutContainer.Q<VisualElement>(nameof(ListViewContainer));

            ScrollView = new ScrollView();

            UIElementsUtility.Runtime.Utility.VisualElementExtensions.AddChild(ListViewContainer, ScrollView); 

            //TOOLBAR
            ToolbarElements = new List<VisualElement>();

            //BACKGROUND COLORS
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleBackgroundColor(LayoutContainer.Q<VisualElement>("Top"), BackgroundColor);
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleBackgroundColor(LayoutContainer.Q<VisualElement>("Bottom"), BackgroundColor);
            
            //TEXT COLORS
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleColor(ListTitle, ListNameTextColor);

            //TEXT FONTS
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleUnityFont(ListTitle, ListNameFont);

            //INJECTIONS
            AddNewItemButtonIsHidden = false;
            AddItemButton.OnClick += () =>
            {
                AddNewItemButtonCallback?.Invoke();
            };

            //LIST VIEW
            PreferredListHeight = ListMinimumHeight;

            VisualUpdate();
        }

        public void AddListViewItems(List<VisualElement> visualElements)
        {
            _listViewItems.AddRange(visualElements);

            for (int i = 0; i < _listViewItems.Count; i++)
            {
                ScrollView.Insert(i, _listViewItems[i]);
            }
        }

        public void Update()
        {
            VisualUpdate();
        }

        #region Visual Update

        private void VisualUpdate()
        {
            MarkDirtyRepaint();
            VisualUpdate_UpdateAddItemButton();
        }

        private void VisualUpdate_UpdateAddItemButton()
        {
            AddItemButton.EnableElement();
            AddItemButton.visible = true;
        }

        #endregion

        public void DisableTopAndBottom()
        {
            LayoutContainer.Q<VisualElement>("Top").RemoveFromHierarchy();
            LayoutContainer.Q<VisualElement>("Bottom").RemoveFromHierarchy();
        }
        
        public ListView SetListTitle(string text)
        {
            ListTitle.text = text;

            bool hasListName = !text.IsNullOrEmpty();
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleDisplay(ListTitle, hasListName ? DisplayStyle.Flex : DisplayStyle.None);
            if (hasListName)
            {
                UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleDisplay(HeaderContainer, DisplayStyle.Flex);
            }

            return this;
        }
        
        public static class Buttons
        {
            private const ElementSize Size = ElementSize.Small;
            private const ButtonStyle ButtonStyle = UIElementsUtility.Runtime.ButtonStyle.Clear;
            internal static Button GetNewToolbarButton(Texture2D texture, string tooltip = "") =>
                UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetTooltip(new Button()
                        .SetIcon(texture)
                        .SetElementSize(Size)
                        .SetButtonStyle(ButtonStyle), tooltip);

            public static Button AddButton => GetNewToolbarButton(GetTexture.Images.Plus, "Add Item").SetAccentColor(AddSelectableColor);
            public static Button RemoveButton => GetNewToolbarButton(GetTexture.Images.Minus, "Remove Item").SetAccentColor(RemoveSelectableColor);
        }
    }
}
#endif
#endif