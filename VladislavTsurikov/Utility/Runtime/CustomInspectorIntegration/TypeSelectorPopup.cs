#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.Utility.Runtime.CustomInspectorIntegration
{
    public class TypeSelectorPopup : EditorWindow
    {
        private const int FontSize = 12;
        private const int Padding = 10;

        private string _title;
        private Action<object> _onSelect;
        private List<object> _items = new();
        private Func<object, string> _displayNameFunc;

        private TextField _searchField;
        private Label _titleLabel;
        private ScrollView _scrollView;

        public static void Open<T>(
            Rect rectActivator,
            string title,
            IEnumerable<T> items,
            Func<T, string> displayNameFunc,
            Action<T> onSelect)
        {
            Rect screenRect = GUIUtility.GUIToScreenRect(rectActivator);

            Vector2 windowSize = new Vector2(
                Mathf.Max(screenRect.width, 300),
                400
            );

            var popup = CreateInstance<TypeSelectorPopup>();
            popup._title = title;
            popup._onSelect = obj => onSelect((T)obj);
            popup._items = new List<object>(items.Cast<object>());
            popup._displayNameFunc = obj => displayNameFunc((T)obj);

            popup.position = new Rect(screenRect.x, screenRect.y + screenRect.height, windowSize.x, windowSize.y);
            popup.ShowAsDropDown(screenRect, windowSize);
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;
            root.style.paddingLeft = Padding;
            root.style.paddingRight = Padding;
            root.style.paddingTop = Padding;
            root.style.paddingBottom = Padding;

            _titleLabel = new Label(_title)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = FontSize + 2,
                    marginTop = Padding,
                    marginBottom = Padding
                }
            };
            root.Add(_titleLabel);

            _searchField = new TextField
            {
                style =
                {
                    fontSize = FontSize
                }
            };
            _searchField.RegisterValueChangedCallback(evt => { UpdateFilteredItems(evt.newValue); });
            root.Add(_searchField);

            _scrollView = new ScrollView
            {
                style =
                {
                    flexGrow = 1,
                    borderTopWidth = 1,
                    borderBottomWidth = 1,
                }
            };
            root.Add(_scrollView);

            UpdateItemList(_items);
        }

        private void UpdateFilteredItems(string searchTerm)
        {
            var filteredItems = string.IsNullOrWhiteSpace(searchTerm)
                ? _items
                : _items.FindAll(item =>
                    _displayNameFunc(item).IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);

            UpdateItemList(filteredItems);
        }

        private void UpdateItemList(IEnumerable<object> items)
        {
            _scrollView.Clear();

            foreach (var item in items)
            {
                var itemName = _displayNameFunc(item);

                var button = new Button(() => OnItemSelected(item))
                {
                    text = itemName,
                    style =
                    {
                        unityTextAlign = TextAnchor.MiddleLeft,
                        fontSize = FontSize,
                        paddingLeft = Padding,
                        paddingRight = Padding,
                        marginTop = 2,
                        marginBottom = 2
                    }
                };

                _scrollView.Add(button);
            }
        }

        private void OnItemSelected(object selectedItem)
        {
            _onSelect?.Invoke(selectedItem);
            Close();
        }
    }
}
#endif