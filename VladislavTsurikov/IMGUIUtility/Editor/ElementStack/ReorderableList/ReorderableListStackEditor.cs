#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.DeepCopy.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    public class ReorderableListStackEditor<T, N> : ComponentStackEditor<T, N>
        where T: Component
        where N: ReorderableListComponentEditor
    {
        private static class Styles
        {
            public static readonly Texture2D Move;
            
            static Styles()
            {
                Move = Resources.Load<Texture2D>("Images/move");
            }
        }

        private UnityEditorInternal.ReorderableList _reorderableList;
        private GUIContent _reorderableListName;
        private bool _dragging;
        private Component _copyComponentElement;
        private bool _displayAddButton;
        
        protected bool ShowActiveToggle = true;
        protected bool RenameSupport = false;
        protected bool CopySettings = false;

        public bool DisplayHeaderText = true;
        public bool DisplayPlusButton = true;

        public ReorderableListStackEditor(GUIContent reorderableListName, AdvancedComponentStack<T> stack, bool displayHeader) : base(stack)
        {
            _reorderableListName = reorderableListName;
            _reorderableList = new UnityEditorInternal.ReorderableList(stack.ReorderableElementList, typeof(T), true, displayHeader, false, false);

            SetupCallbacks();
        }

        protected virtual void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var settingsType in GetSettingsTypeForAddManu())
            {
                if (settingsType.GetAttribute(typeof(DontCreateAttribute)) != null)
                {
                    continue;
                }
                
                if (settingsType.GetAttribute<DontShowInAddMenuAttribute>() != null) 
                {
                    continue;
                }
                
                string context = settingsType.GetAttribute<NameAttribute>().Name;

                if (Stack is ComponentStackSupportSameType<T> componentStackWithSameTypes)
                {
                    menu.AddItem(new GUIContent(context), false, () => componentStackWithSameTypes.CreateComponent(settingsType));
                }
                else if (Stack is ComponentStackOnlyDifferentTypes<T> componentStackWithDifferentTypes)
                {
                    bool exists = componentStackWithDifferentTypes.HasType(settingsType);

                    if (!exists)
                    {
                        menu.AddItem(new GUIContent(context), false, () => componentStackWithDifferentTypes.CreateIfMissingType(settingsType));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(context));
                    }
                }
            }

            menu.ShowAsContext();
        }
        
        protected virtual void AddCB(UnityEditorInternal.ReorderableList list)
        {
            ShowAddMenu();
        }

        protected virtual void DrawHeaderElement(Rect headerRect, int index, N componentEditor)
        {
            if (ShowActiveToggle && componentEditor.Target.ShowActiveToggle())
            {
                bool temporaryActive = ((Component)componentEditor.Target).Active;
                
                componentEditor.Target.SelectSettingsFoldout = CustomEditorGUI.HeaderWithMenu(headerRect,
                    componentEditor.Target.Name,
                    componentEditor.Target.SelectSettingsFoldout, ref temporaryActive, () => Menu(Stack.ElementList[index], index));

                ((Component)componentEditor.Target).Active = temporaryActive;
            }
            else
            {
                componentEditor.Target.SelectSettingsFoldout = CustomEditorGUI.HeaderWithMenu(headerRect,
                    componentEditor.Target.Name,
                    componentEditor.Target.SelectSettingsFoldout, () => Menu(Stack.ElementList[index], index));
            }
        }
        
        protected virtual void Menu(T component, int index)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => Stack.Reset(index));

            if (component.IsDeletable())
            {
                menu.AddItem(new GUIContent("Remove"), false, () => Stack.Remove(index));
            }

            if (RenameSupport)
            {
                menu.AddSeparator ("");
                menu.AddItem(new GUIContent("Rename"), component.Renaming, () => RenameComponent(component));
            }

            if (CopySettings)
            {
                menu.AddSeparator ("");
                menu.AddItem(new GUIContent("Copy Settings"), false, () => _copyComponentElement = DeepCopier.Copy(component));

                if (_copyComponentElement != null)
                {
                    menu.AddItem(new GUIContent("Paste Settings"), false, () => Stack.ReplaceElement((T)DeepCopier.Copy(_copyComponentElement), index));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste Settings"), false);
                }
            }

            menu.ShowAsContext();
        }

        private void SetupCallbacks()
        {
            if(_reorderableListName != null) _reorderableList.drawHeaderCallback = DrawHeaderCB;
            _reorderableList.drawElementCallback = DrawElementCB;
            _reorderableList.elementHeightCallback = ElementHeightCB;
            _reorderableList.onAddCallback = AddCB;
            _reorderableList.onRemoveCallback = RemoveElement;
            _reorderableList.onChangedCallback = OnChangedCallback;
        }

        private void OnChangedCallback(UnityEditorInternal.ReorderableList list)
        {
            Stack.IsDirty = true;
        }

        public virtual void OnGUI()
		{
            if(Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }
            
            Rect rect = EditorGUILayout.GetControlRect(true, _reorderableList.GetHeight());
            rect = EditorGUI.IndentedRect(rect);
                
            _reorderableList.DoList(rect);
        }
        
        public void OnGUI(Rect rect)
        {
            if(Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();  
                RefreshEditors();
                Stack.IsDirty = false;
            }

            OnReorderableListStackGUI(rect);
        }

        protected virtual void OnReorderableListStackGUI(Rect rect)
        {
            _reorderableList.DoList(rect);
        }

        public virtual float GetElementStackHeight()
        {
            float height = CustomEditorGUI.SingleLineHeight * 3;
            
            for (int i = 0; i < Editors.Count; i++)
            {
                height += CustomEditorGUI.SingleLineHeight;

                if (Editors[i].Target.Renaming)
                {
                    continue;
                }

                if (Editors[i].Target.SelectSettingsFoldout)
                {
                    height += Editors[i].GetElementHeight(i);
                }
            }

            return height;
        }

        private void RemoveElement(UnityEditorInternal.ReorderableList list)
        {
            Stack.Remove(list.index);
        }

        private void DrawHeaderCB(Rect rect)
        {
            if(DisplayHeaderText)
                CustomEditorGUI.Label(rect, _reorderableListName.text, CustomEditorGUI.GetStyle(StyleName.LabelFoldout));
            
            if(DisplayPlusButton)
                DrawPlusButton(rect);
        }
        
        private void DrawPlusButton(Rect rect)
        {
            Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight - 3;
			
            Color color = GUI.color;
            var menuRect = new Rect(buttonRect.x, buttonRect.y + 2f, 14, 14);
            
            if (CustomEditorGUI.DrawIcon(menuRect, StyleName.IconButtonPlus, EditorColors.Instance.Green))
            {
                AddCB(_reorderableList);
                Event.current.Use();
            }
            
            GUI.color = color;
        }

        private float ElementHeightCB(int index)
        {
            if (Editors.Count == 0)
            {
                return 0;
            }
            
            try
            {
                N editor = Editors[index];
            }
            catch (Exception e)
            {
                Debug.Log(Editors.Count);
                Debug.Log("index " + index);
                throw;
            }
            
            N componentEditor = Editors[index];

            float height = EditorGUIUtility.singleLineHeight * 1.5f;

            if(componentEditor == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if(!componentEditor.Target.SelectSettingsFoldout)
            {
                return EditorGUIUtility.singleLineHeight + 5;
            }
            else
            {
                if (RenameSupport)
                {
                    if (componentEditor.Target.Renaming)
                    {
                        return EditorGUIUtility.singleLineHeight + 5;
                    }
                    else
                    {
                        height += componentEditor.GetElementHeight(index);
                        return height;
                    }
                }
                else
                {
                    height += componentEditor.GetElementHeight(index);
                    return height;
                }
            }
        }

        private void DrawElementCB(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            if (Editors.Count == 0)
            {
                return;
            }
            
            if (Stack.IsDirty)
            {
                RefreshEditors();
            }

            float dividerSize = 1f;
            float paddingV = 6f;
            float paddingH = 4f;
            float iconSize = 14f;

            bool isSelected = _reorderableList.index == index;

            Color bgColor;

            if(EditorGUIUtility.isProSkin)
            {
                if(isSelected)
                    UnityEngine.ColorUtility.TryParseHtmlString("#424242", out bgColor);
                else
                    UnityEngine.ColorUtility.TryParseHtmlString("#383838", out bgColor);
            }
            else
            {
                if(isSelected)
                    UnityEngine.ColorUtility.TryParseHtmlString("#b4b4b4", out bgColor);
                else
                    UnityEngine.ColorUtility.TryParseHtmlString("#c2c2c2", out bgColor);
            }

            Color dividerColor;

            if(isSelected)
            {
                dividerColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                if(EditorGUIUtility.isProSkin)
                    UnityEngine.ColorUtility.TryParseHtmlString("#202020", out dividerColor);
                else
                    UnityEngine.ColorUtility.TryParseHtmlString("#a8a8a8", out dividerColor);
            }

            Color prevColor = GUI.color;

            // modify total rect so it hides the builtin list UI
            totalRect.xMin -= 20f;
            totalRect.xMax += 4f;
            
            bool containsMouse = totalRect.Contains(Event.current.mousePosition);

            // modify currently selected element if mouse down in this elements GUI rect
            if(containsMouse && Event.current.type == EventType.MouseDown)
            {
                _reorderableList.index = index;
            }

            // draw list element separator
            Rect separatorRect = totalRect;
            // separatorRect.height = dividerSize;
            GUI.color = dividerColor;
            GUI.DrawTexture(separatorRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = prevColor;

            // Draw BG texture to hide ReorderableList highlight
            totalRect.yMin += dividerSize;
            totalRect.xMin += dividerSize;
            totalRect.xMax -= dividerSize;
            totalRect.yMax -= dividerSize;
            
            GUI.color = bgColor;
            GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);

            GUI.color = new Color(.7f, .7f, .7f, 1f);

            N componentEditor = Editors[index];

            if(componentEditor == null)
            {
                return;
            }
            
            if (componentEditor.Target.Renaming)
            {
                GUI.color = new Color(1f, 1f, 1f, 1f);

                const float plusRectX = 5;

                totalRect.x += plusRectX;
                totalRect.y += 2;
                totalRect.width -= plusRectX + 15;
                totalRect.height = EditorGUIUtility.singleLineHeight;
                
                RenameComponentGUI(totalRect, componentEditor.Target);

                GUI.color = prevColor;
                return;
            }

            Rect moveRect = new Rect(totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize );

            // draw move handle rect
            EditorGUIUtility.AddCursorRect(moveRect, MouseCursor.Pan);
            GUI.DrawTexture(moveRect, Styles.Move, ScaleMode.StretchToFill);

            Rect headerRect = totalRect;
            
            headerRect.x += 15;
            headerRect.height = EditorGUIUtility.singleLineHeight * 1.3f;

            GUI.color = new Color(1f, 1f, 1f, 1f);

            DrawHeaderElement(headerRect, index, componentEditor);
            
            // update dragging state
            if(containsMouse && isSelected)
            {
                if (Event.current.type == EventType.MouseDrag && !_dragging && isFocused)
                {
                    _dragging = true;
                    _reorderableList.index = index;
                }
            }

            if(_dragging)
            {
                if(Event.current.type == EventType.MouseUp)
                {
                    _dragging = false;
                }
            }

            using(new EditorGUI.DisabledScope(!Stack.ElementList[index].Active))
            {
                float rectX = 35;

                totalRect.x += rectX;
                totalRect.y += EditorGUIUtility.singleLineHeight + 3;
                totalRect.width -= rectX + 15;
                totalRect.height = EditorGUIUtility.singleLineHeight;

                GUI.color = prevColor;

                if(componentEditor.Target.SelectSettingsFoldout)
                {
                    try
                    {
                        componentEditor.OnGUI(totalRect, index);
                    }
                    catch
                    {
                        // EditorApplication.delayCall += () =>
                        // {
                        //     Stack.Remove(index);
                        // };
                        //
                        // Debug.LogError("ComponentEditor has an error! It was deleted automatically");
                    }
                }
            }

            GUI.color = prevColor;
        }

        private void RenameComponent(Component componentElement)
        {
            componentElement.Renaming = !componentElement.Renaming;
            componentElement.RenamingName = componentElement.Name;
        }

        private void RenameComponentGUI(Rect rect, Element stackElement) 
        {
            if (stackElement.Renaming == false)
            {
                return;
            }
			
            Color initialGUIColor = GUI.color;
            
            Rect rectField = CustomEditorGUI.PrefixLabel(rect, new GUIContent("Rename to"));

            rectField.x -= 28;
            
            GUI.color = EditorColors.Instance.orangeNormal;
            stackElement.RenamingName = EditorGUI.TextField(rectField, GUIContent.none, stackElement.RenamingName);
            GUI.color = initialGUIColor;
            
            Rect iconRect = new Rect(rectField.x + rectField.width + 2, rectField.y + 1, 14, 14);

            if (CustomEditorGUI.DrawIcon(iconRect, StyleName.IconButtonOk, EditorColors.Instance.Green) || Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp) //rename OK button
            {
                stackElement.Renaming = false;
                stackElement.Name = stackElement.RenamingName;

                Event.current.Use();
            }

            iconRect.x += 16;

            if (CustomEditorGUI.DrawIcon(iconRect, StyleName.IconButtonCancel, EditorColors.Instance.Red)) //rename CANCEL button
            {
                stackElement.RenamingName = stackElement.Name;
                stackElement.Renaming = false;

                Event.current.Use();
            }
			
            GUI.color = initialGUIColor;
			
            GUILayout.Space(15);
        }

        protected List<Type> GetSettingsTypeForAddManu()
        {
            List<Type> settingsTypes = new List<Type>();

            foreach (var type in AllEditorTypes<T>.Types)
            {
                Type settingsType = type.Key;
                
                if (type.Value.GetAttribute<DontDrawForAddButton>() != null)
                {
                    continue;
                }

                settingsTypes.Add(settingsType);
                
            }

            return settingsTypes;
        }
    }
}
#endif