#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem.Components;
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;
using Object = System.Object;

namespace VladislavTsurikov.SceneManagerTool.Editor.BuildSceneCollectionSystem
{
    public class BuildSceneCollectionStackEditor : TabComponentStackEditor<BuildSceneCollection, DefaultBuildSceneCollectionEditor>
    {
        private BuildSceneCollectionList _buildSceneCollectionList;
        
        public BuildSceneCollectionStackEditor(BuildSceneCollectionList list) : base(list)
        {
            _buildSceneCollectionList = (BuildSceneCollectionList)Stack;

            _tabStackEditor.AddCallback = ShowAddManu;
            _tabStackEditor.IconColor = SetTabColor;
            _tabStackEditor.TabWidthFromName = true;
            _tabStackEditor.EnableRename = true;
        }

        public override void OnTabStackGUI()
        {
            _tabStackEditor.OnGUI();

            if (Stack.ElementList.Count == 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Click on the plus button to add Build Scene Collection");
                EditorGUILayout.EndVertical();
            }
            else if(Stack.SelectedElement == null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("No Build Scene Collection Selected");
                EditorGUILayout.EndVertical();
            }
        }
        
        protected override GenericMenu TabMenu(int currentTabIndex)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, GUIUtility.ContextMenuCallback, new Action(() => {Stack.Remove(currentTabIndex);}));
            
            menu.AddSeparator("");
            
            menu.AddItem(new GUIContent("Active"), _buildSceneCollectionList.ActiveBuildSceneCollection == Stack.ElementList[currentTabIndex], GUIUtility.ContextMenuCallback, new Action(() =>
            {
                BuildSceneCollectionList buildSceneCollectionList = (BuildSceneCollectionList)Stack;
                buildSceneCollectionList.ActiveBuildSceneCollection = Stack.ElementList[currentTabIndex];
            }));

            return menu;
        }

        protected override void ShowAddManu()
        {
            GenericMenu menu = new GenericMenu();

            Dictionary<Type, Type> types = AllEditorTypes<BuildSceneCollection>.Types;

            if (types.Count == 1)
            {
                EditorApplication.delayCall += () =>
                {
                    _buildSceneCollectionList.CreateComponent(typeof(DefaultBuildSceneCollection));
                };
            }
            else
            {
                foreach (var type in AllEditorTypes<BuildSceneCollection>.Types)
                {
                    Type settingsType = type.Key;
                    
                    if(settingsType.ToString() == typeof(DefaultBuildSceneCollection).ToString())
                        continue;
                
                    string context = settingsType.GetAttribute<MenuItemAttribute>().Name;

                    menu.AddItem(new GUIContent(context), false, () => _buildSceneCollectionList.CreateComponent(settingsType));
                }
                
                menu.AddSeparator("");
                
                menu.AddItem(new GUIContent("Create Default"), false, () => _buildSceneCollectionList.CreateComponent(typeof(DefaultBuildSceneCollection)));
            }

            menu.ShowAsContext();
        }
        
        private void SetTabColor(Object tab, out Color barColor, out Color labelColor)
        {
            BuildSceneCollection buildSceneCollection = (BuildSceneCollection)tab;
            barColor = EditorColors.Instance.LabelColor;

            if(_buildSceneCollectionList.ActiveBuildSceneCollection != buildSceneCollection)
            {
                if(buildSceneCollection.Selected)
                {
                    labelColor = EditorColors.Instance.orangeDark;
                    barColor = EditorColors.Instance.orangeNormal;
                }
                else
                {
                    labelColor = EditorColors.Instance.orangeNormal;
                    barColor = EditorColors.Instance.orangeDark.WithAlpha(0.3f);
                }
            }
            else
            {
                labelColor = EditorColors.Instance.LabelColor;

                if(buildSceneCollection.Selected)
                {
                    barColor = EditorColors.Instance.ToggleButtonActiveColor;
                }
                else
                {
                    barColor = EditorColors.Instance.ToggleButtonInactiveColor;
                }
            }
        }
    }
}
#endif