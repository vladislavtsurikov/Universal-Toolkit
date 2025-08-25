﻿#if UNITY_EDITOR
#if !DISABLE_VISUAL_ELEMENTS
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.UIElementsUtility;
using VladislavTsurikov.UIElementsUtility.Runtime.Utility;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors;
using VladislavTsurikov.UIElementsUtility.Runtime;
using VisualElementExtensions = VladislavTsurikov.UIElementsUtility.Runtime.Utility.VisualElementExtensions;

namespace VladislavTsurikov.SceneDataSystem.Editor.VisualElements
{
    public class Button : VisualElement
    {
        // public new class UxmlFactory : UxmlFactory<Button, UxmlTraits> { }
        // public new class UxmlTraits : VisualElement.UxmlTraits { }
        
        public TemplateContainer TemplateContainer { get; }
        public VisualElement LayoutContainer { get; }
        public VisualElement ButtonContainer { get; }
        public Image ButtonIcon { get; }
        public Label ButtonLabel { get; }
        
        public UnityAction OnClick;
        public Element Element { get; }
        private static Font Font => GetFont.Ubuntu.Light;
        
        public SelectionState SelectionState
        {
            get => Element.SelectionState;
            set => Element.SelectionState = value;
        }
        
        #region IconType

        private IconType IconType { get; set; } = IconType.None;
        private List<VisualElement> IconDependentComponents { get; }
        internal void UpdateIconType(IconType value)
        {
            if (IconType != IconType.None)
            {
                UIElementsUtility.Runtime.Utility.UIElementsUtility.RemoveClass(IconType.ToString(), IconDependentComponents);
            }

            if (value != IconType.None)
            {
                UIElementsUtility.Runtime.Utility.UIElementsUtility.AddClass(value.ToString(), IconDependentComponents);
            }

            IconType = value;
        }

        #endregion
        
        #region ElementSize

        private ElementSize ElementSize { get; set; }
        private List<VisualElement> ElementSizeDependentElements { get; }
        public Button SetElementSize(ElementSize value)
        {
            UIElementsUtility.Runtime.Utility.UIElementsUtility.RemoveClass(ElementSize.ToString(), ElementSizeDependentElements);
            UIElementsUtility.Runtime.Utility.UIElementsUtility.AddClass(value.ToString(), ElementSizeDependentElements);
            ElementSize = value;
            return this;
        }
        public Button ResetElementSize() =>
            SetElementSize(ElementSize.Normal);

        #endregion
        
        #region LayoutOrientation

        private LayoutOrientation LayoutOrientation { get; set; }
        private List<VisualElement> LayoutOrientationDependentElements { get; }
        public Button SetLayoutOrientation(LayoutOrientation value)
        {
            UIElementsUtility.Runtime.Utility.UIElementsUtility.RemoveClass(LayoutOrientation.ToString(), LayoutOrientationDependentElements);
            UIElementsUtility.Runtime.Utility.UIElementsUtility.AddClass(value.ToString(), LayoutOrientationDependentElements);
            LayoutOrientation = value;
            return this;
        }
        public Button ResetLayoutOrientation() =>
            SetLayoutOrientation(LayoutOrientation.Horizontal);

        #endregion
        
        public Button()
        {
            Add(TemplateContainer = GetLayout.VisualElements.Button.CloneTree());
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.AddStyle(TemplateContainer, GetStyle.VisualElements.Button);
            
            LayoutContainer = TemplateContainer.Q<VisualElement>(nameof(LayoutContainer));
            ButtonContainer = LayoutContainer.Q<VisualElement>(nameof(ButtonContainer));
            ButtonIcon = LayoutContainer.Q<Image>(nameof(ButtonIcon));
            ButtonLabel = LayoutContainer.Q<Label>(nameof(ButtonLabel));

            ElementSizeDependentElements = new List<VisualElement> { LayoutContainer, ButtonContainer, ButtonIcon, ButtonLabel };
            LayoutOrientationDependentElements = new List<VisualElement>(ElementSizeDependentElements);
            ButtonStyleDependentElements = new List<VisualElement>(ElementSizeDependentElements);
            IconDependentComponents = new List<VisualElement>(ElementSizeDependentElements);

            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleUnityFont(ButtonLabel, Font);

            Element = new Element(this)
            {
                OnStateChanged = StateChanged,
                OnClick = ExecuteOnClick,
                OnPointerEnter = ExecuteOnPointerEnter
            };

            //RESET
            {
                ResetElementSize();
                ResetLayoutOrientation();
                ResetButtonStyle();
                UIElementsUtility.Runtime.Utility.VisualElementExtensions.ResetLayout(this);
                this.ResetAccentColor();
                this.ClearIcon();
                this.ClearLabelText();
                this.ClearOnClick();
                UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetTooltip(this, string.Empty);
                SelectionState = SelectionState.Normal;
            }
        }

        #region ButtonStyle

        private ButtonStyle ButtonStyle { get; set; }
        private List<VisualElement> ButtonStyleDependentElements { get; }
        public Button SetButtonStyle(ButtonStyle value)
        {
            UIElementsUtility.Runtime.Utility.UIElementsUtility.RemoveClass(ButtonStyle.ToString(), ButtonStyleDependentElements);
            UIElementsUtility.Runtime.Utility.UIElementsUtility.AddClass(value.ToString(), ButtonStyleDependentElements);
            ButtonStyle = value;
            Element.StateChanged();
            return this;
        }
        
        public Button ResetButtonStyle() =>
            SetButtonStyle(ButtonStyle.Clear);

        #endregion

        public void Reset()
        {
            ResetElementSize();
            ResetLayoutOrientation();
            ResetButtonStyle();

            this.SetEnabled(true);
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.ResetLayout(this);
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetTooltip(this, string.Empty);
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetName(this, string.Empty);
            this.ResetAccentColor();
            this.ClearIcon();
            this.ClearLabelText();
            //this.ClearInfoContainer();
            this.ClearOnClick();
            this.SetSelectionState(SelectionState.Normal);
            
            
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleTextAlign(ButtonLabel, TextAnchor.MiddleCenter);
        }
        
        private void StateChanged()
        {
            switch (ButtonStyle)
            {
                case ButtonStyle.Contained:

                    break;
                case ButtonStyle.Outline:
                    Element.ContainerColor = Color.clear;
                    Element.ContainerBorderColor = Element.TextColor.WithAlpha(Element.TextColor.Alpha() * 0.3f);
                    break;
                case ButtonStyle.Clear:
                    if (SelectionState == SelectionState.Normal)
                    {
                        Element.IconColor = Element.IconSelectableColor.GetColor(SelectionState);
                    }

                    Element.ContainerColor = Color.clear;
                    Element.ContainerBorderColor = Color.clear;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleBackgroundImageTintColor(ButtonIcon, Element.IconColor); //Icon
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleColor(ButtonLabel, Element.TextColor); //Label
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleBackgroundColor(ButtonContainer, Element.ContainerColor); //Background
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleBorderColor(LayoutContainer, Element.ContainerBorderColor); //Border
        }

        public void ExecuteOnClick(EventBase clickEvent = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }

            OnClick?.Invoke();
            //if (animationTrigger == ButtonAnimationTrigger.OnClick)
                //iconReaction?.Play();
        }

        public void ExecuteOnPointerEnter(PointerEnterEvent enterEvent = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }
            //if (animationTrigger == ButtonAnimationTrigger.OnPointerEnter)
                //iconReaction?.Play();
        }
    }

    public static class ButtonExtensions
    {
        /// <summary> Set the button's selection state </summary>
        /// <param name="target"> Target button </param>
        /// <param name="state"> New selection state </param>
        public static T SetSelectionState<T>(this T target, SelectionState state) where T : Button
        {
            target.SelectionState = state;
            return target;
        }
        
        /// <summary> Enable button and update its visual state </summary>
        /// <param name="target"> Target button </param>
        public static T EnableElement<T>(this T target) where T : Button
        {
            target.Element.Enable();
            return target;
        }

        /// <summary> Disable button and updates its visual state </summary>
        /// <param name="target"> Target Button </param>
        public static T DisableElement<T>(this T target) where T : Button
        {
            target.Element.Disable();
            return target;
        }
        
        #region Label

        /// <summary> Set label text </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="labelText"> Label text </param>
        public static T SetLabelText<T>(this T target, string labelText) where T : Button
        {
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleDisplay(target.ButtonLabel
                    .SetText(labelText), String.IsNullOrEmpty(labelText) ? DisplayStyle.None : DisplayStyle.Flex);
            return target;
        }

        /// <summary> Clear the text and tooltip values from the button's label </summary>
        public static T ClearLabelText<T>(this T target) where T : Button =>
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetTooltip(target.SetLabelText(string.Empty), string.Empty);

        #endregion

        #region OnClick

        /// <summary> Set callback for OnClick (removes any other callbacks set to OnClick) </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="callback"> OnClick callback </param>
        public static T SetOnClick<T>(this T target, UnityAction callback) where T : Button
        {
            if (callback == null)
            {
                return target;
            }

            target.OnClick = callback;
            return target;
        }

        /// <summary> Add callback to OnClick (adds another callback to OnClick) </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="callback"> OnClick callback </param>
        public static T AddOnClick<T>(this T target, UnityAction callback) where T : Button
        {
            if (callback == null)
            {
                return target;
            }

            target.OnClick += callback;
            return target;
        }

        /// <summary> Clear any callbacks set to OnClick </summary>
        /// <param name="target"> Target Button</param>
        public static T ClearOnClick<T>(this T target) where T : Button
        {
            target.OnClick = null;
            return target;
        }

        #endregion
        
        #region Icon

        /// <summary> Set Static Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="iconTexture2D"> Icon texture </param>
        public static T SetIcon<T>(this T target, Texture2D iconTexture2D) where T : Button
        {
            target.UpdateIconType(IconType.Static);
            //target.iconReaction?.Recycle();
            //target.iconReaction = null;
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleBackgroundImage(target.ButtonIcon, iconTexture2D);
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleDisplay(target.ButtonIcon, DisplayStyle.Flex);
            //target.SetAnimationTrigger(ButtonAnimationTrigger.None);
            return target;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        /// <param name="target"> Target Button </param>
        public static T ClearIcon<T>(this T target) where T : Button
        {
            target.UpdateIconType(IconType.None);
            //target.iconReaction?.Recycle();
            //target.iconReaction = null;
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleBackgroundImage(target.ButtonIcon, (Texture2D)null);
            UIElementsUtility.Runtime.Utility.VisualElementExtensions.SetStyleDisplay(target.ButtonIcon, DisplayStyle.None);
            return target;
        }

        #endregion
        
        #region Accent Color

        /// <summary> Set button's accent color </summary>
        /// <param name="target"> Target button </param>
        /// <param name="value"> New accent color </param>
        public static T SetAccentColor<T>(this T target, EditorSelectableColorInfo value) where T : Button
        {
            target.Element.SetAccentColor(value);
            return target;
        }

        /// <summary> Reset the accent color to its default value </summary>
        /// <param name="target"> Target Button </param>
        public static T ResetAccentColor<T>(this T target) where T : Button
        {
            target.Element.ResetAccentColor();
            return target;
        }

        #endregion
    }
}
#endif
#endif