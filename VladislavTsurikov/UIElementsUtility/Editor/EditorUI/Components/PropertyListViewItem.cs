﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Events;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Components
{
    public class PropertyListViewItem : ListViewItem
    {
        public PropertyField PropertyField { get; private set; }
        public UnityAction<SerializedProperty> OnRemoveButtonClick;

        public PropertyListViewItem(ListView listView) : base(listView)
        {
            ItemContentContainer.Clear();
            ItemContentContainer.Add(PropertyField = new PropertyField());
        }

        public void Update(int index, SerializedProperty property)
        {
            //UPDATE INDEX
            ShowItemIndex = ListView.ShowItemIndex;
            UpdateItemIndex(index);

            //UPDATE PROPERTY
            PropertyField.BindProperty(property);

            //UPDATE REMOVE BUTTON
            ItemRemoveButton.OnClick = () => OnRemoveButtonClick?.Invoke(property);
        }
    }
}
#endif