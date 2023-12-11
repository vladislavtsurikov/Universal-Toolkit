#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Colors;
using VladislavTsurikov.Utility.Runtime.Extensions;
using ButtonStyle = VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Enums.ButtonStyle;
using ElementSize = VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Enums.ElementSize;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Components
{
#if !DISABLE_UIELEMENTS
    public class ListView : VisualElement
    {
        private const int ListMinimumHeight = 120;
        private List<VisualElement> _listViewItems = new List<VisualElement>();
        
        //ACTIONS
        public UnityAction AddNewItemButtonCallback;

        //REFERENCES
        public TemplateContainer TemplateContainer { get; }
        public VisualElement LayoutContainer { get; }
        public VisualElement HeaderContainer { get; }
        public Label ListTitle { get; }
        public VisualElement ListViewContainer { get; }
        public ScrollView ScrollView;

        public List<VisualElement> ToolbarElements { get; }
        
        
        //BUTTONS
        private Button _mAddItemButton;
        public Button AddItemButton => _mAddItemButton ??= Buttons.AddButton;

        //SETTINGS
        public bool ShowItemIndex { get; private set; }
        public int PreferredListHeight { get; private set; }
        public bool AddNewItemButtonIsHidden { get; private set; }

        //SELECTABLE COLORS
        private static EditorSelectableColorInfo ActionSelectableColor => EditorSelectableColors.Default.Action;
        private static EditorSelectableColorInfo AddSelectableColor => EditorSelectableColors.Default.Add;
        private static EditorSelectableColorInfo RemoveSelectableColor => EditorSelectableColors.Default.Remove;

        //COLORS
        private static Color ListNameTextColor => EditorColors.Default.TextTitle;
        private static Color BackgroundColor => EditorColors.Default.Background;

        //FONTS
        private static Font ListNameFont => EditorFonts.Ubuntu.Light;

        public ListView()
        {
            Add(TemplateContainer = EditorLayouts.EditorUI.ListView.CloneTree());
            TemplateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.EditorUI.ListView);

            //REFERENCES
            LayoutContainer = TemplateContainer.Q<VisualElement>(nameof(LayoutContainer));
            HeaderContainer = LayoutContainer.Q<VisualElement>(nameof(HeaderContainer));
            ListTitle = HeaderContainer.Q<Label>(nameof(ListTitle)); 
            ListViewContainer = LayoutContainer.Q<VisualElement>(nameof(ListViewContainer));

            ScrollView = new ScrollView();

            ListViewContainer.AddChild(ScrollView); 

            //TOOLBAR
            ToolbarElements = new List<VisualElement>();

            //BACKGROUND COLORS
            LayoutContainer.Q<VisualElement>("Top").SetStyleBackgroundColor(BackgroundColor);
            LayoutContainer.Q<VisualElement>("Bottom").SetStyleBackgroundColor(BackgroundColor);

            //TEXT COLORS
            ListTitle.SetStyleColor(ListNameTextColor);

            //TEXT FONTS
            ListTitle.SetStyleUnityFont(ListNameFont);

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
            ListTitle.SetStyleDisplay(hasListName ? DisplayStyle.Flex : DisplayStyle.None);
            if (hasListName) HeaderContainer.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }
        
        public static class Buttons
        {
            private const ElementSize Size = ElementSize.Small;
            private const ButtonStyle ButtonStyle = Enums.ButtonStyle.Clear;
            private static EditorSelectableColorInfo AccentColor => ActionSelectableColor;

            internal static Button GetNewToolbarButton(Texture2D texture, string tooltip = "") =>
                new Button()
                    .SetIcon(texture)
                    .SetElementSize(Size)
                    .SetButtonStyle(ButtonStyle)
                    .SetAccentColor(AccentColor)
                    .SetTooltip(tooltip);

            public static Button AddButton => GetNewToolbarButton(EditorTextures.Images.Plus, "Add Item").SetAccentColor(AddSelectableColor);
            public static Button RemoveButton => GetNewToolbarButton(EditorTextures.Images.Minus, "Remove Item").SetAccentColor(RemoveSelectableColor);
        }
    }
#endif
}
#endif